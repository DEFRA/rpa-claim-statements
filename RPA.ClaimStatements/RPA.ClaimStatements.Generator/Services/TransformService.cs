using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Data.Entities.CS;
using System.Diagnostics;
using RPA.ClaimStatements.Data.Context;

namespace RPA.ClaimStatements.Generator.Services
{
    public class TransformService : ITransformService, IDisposable
    {
        ClaimStatementsContext db;
        IFileService fileService;
        IDateService dateService;
        IFolderService folderService;
        IConfigurationService configurationService;
        ILogService logService;

        string root;
        List<Transformation> transformations;
        string outboundFolder;
        string archiveFolder;
        bool archive;

        public TransformService(string root)
        {
            this.db = new ClaimStatementsContext();
            this.fileService = new FileService();
            this.dateService = new DateService();
            this.folderService = new FolderService(db);
            this.configurationService = new ConfigurationService(db);
            this.logService = new LogService(db);            
            this.root = root;

            this.transformations = db.Transformations.AsNoTracking().ToList();
            this.outboundFolder = folderService.GetPath("PDF Outbound");
            this.archiveFolder = folderService.GetPath("PDF Archive");
            this.archive = configurationService.IsActive("Archive");
        }

        public TransformService(ClaimStatementsContext context, string root)
        {
            this.db = context;
            this.fileService = new FileService();
            this.dateService = new DateService();
            this.folderService = new FolderService(context);
            this.configurationService = new ConfigurationService(context);
            this.logService = new LogService(context);            
            this.root = root;

            this.transformations = context.Transformations.AsNoTracking().ToList();
            this.outboundFolder = folderService.GetPath("PDF Outbound");
            this.archiveFolder = folderService.GetPath("PDF Archive");
            this.archive = configurationService.IsActive("Archive");
        }

        public TransformService(ClaimStatementsContext context, IFileService fileService, IDateService dateService, IFolderService folderService, 
            IConfigurationService configurationService, ILogService logService, string root)
        {
            this.db = context;
            this.fileService = fileService;
            this.dateService = dateService;
            this.folderService = folderService;
            this.configurationService = configurationService;
            this.logService = logService;
            this.root = root;
            
            this.transformations = context.Transformations.AsNoTracking().ToList();
            this.outboundFolder = folderService.GetPath("PDF Outbound");
            this.archiveFolder = folderService.GetPath("PDF Archive");
        }

        public void Transform(Request request, ClaimStatement statement, string statementType)
        {
            string xmlFilePath = ConvertToXML(statement, statementType);

            Transformation transformation = transformations.Where(x => x.StatementType == statementType).FirstOrDefault();
            
            XmlDocument newXml = TransformXML(xmlFilePath, transformation.XSLT);

            int attempt = 1;
            bool retry = true;
            string docxFilePath = null;
            string pdfFilePath = null;

            while (retry)
            {
                if (attempt <= 3)
                {
                    try
                    {
                        docxFilePath = CopyTemplate(request.Claim.ClaimID, transformation.Template);
                        
                        CreateDOCX(docxFilePath, newXml, request.Claim.SUM.SBI, statementType);
                        
                        pdfFilePath = ConvertToPDF(docxFilePath, request.Claim.SUM.FRN, request.Claim.SUM.SBI, request.Claim.SUM.SchemeYear);
                        retry = false;                  
                    }
                    catch (Exception)
                    {
                        attempt++;
                    }
                }
                else
                {
                    retry = false;
                    throw new ClaimStatementGenerationException("Unable to convert statement to PDF after " + (attempt - 1).ToString() + " attempts.");
                }
            }

            fileService.Delete(docxFilePath);
            fileService.Delete(xmlFilePath);
            
            string archiveFilePath = "Not Archived";

            if (archive)
            {
                try
                {
                    archiveFilePath = fileService.Archive(pdfFilePath, archiveFolder);
                }
                catch(Exception ex)
                {
                    throw new ClaimStatementGenerationException(string.Format("Unable to archive statement - {0}", ex.Message));
                }
            }

            archiveFilePath = archiveFilePath.Replace(@"\\", @"\");

            logService.Log(request.Claim.SUM.FRN, request.Claim.SUM.SBI, request.Claim.SUM.SchemeYear, archiveFilePath, request.Claim.ClaimID);

            try
            {
                Publish(pdfFilePath);
            }
            catch (Exception ex)
            {
                throw new ClaimStatementGenerationException(string.Format("Unable to publish statement - {0}", ex.Message));
            }

        }

        public string ConvertToPDF(string filePath, long frn, int sbi, int schemeYear)
        {
            string pdfFilePath = Path.Combine(root, "PDF", string.Format("RPA_OUT_BPS{0}_ClmStmt_{1}_{2}_{3}.pdf", schemeYear, frn, sbi, dateService.CurrentDateTimeString()));

            Application wordApp = null;
            Document wordDocument = null;

            object paramMissing = Type.Missing;

            wordApp = new Application();

            wordApp.DisplayAlerts = WdAlertLevel.wdAlertsNone;

            wordDocument = wordApp.Documents.Open(filePath, ReadOnly: true, Visible: false);

            wordApp.ActivePrinter = "Microsoft XPS Document Writer on Ne00:";

            wordDocument.ExportAsFixedFormat(pdfFilePath, WdExportFormat.wdExportFormatPDF);

            wordDocument.Close(false, paramMissing, paramMissing);

            wordApp.Quit();

            return pdfFilePath;
        }

        public string ConvertToXML(ClaimStatement statement, string statementType)
        {
            string filePath = null;

            switch(statementType)
            {
                case "BPS":
                    filePath = Path.Combine(root, "XML", string.Format("{0}{1}.xml", Guid.NewGuid(), dateService.CurrentDateTimeString()));

                    XmlSerializer writer = new XmlSerializer(typeof(ClaimStatement));

                    using (StreamWriter file = new StreamWriter(filePath))
                    {
                        writer.Serialize(file, statement);
                    }
                    break;
                case "XB":
                    filePath = Path.Combine(root, "XML", string.Format("{0}{1}.xml", Guid.NewGuid(), dateService.CurrentDateTimeString()));

                    XmlSerializer writerXB = new XmlSerializer(typeof(ClaimStatementXB));

                    using (StreamWriter file = new StreamWriter(filePath))
                    {
                        writerXB.Serialize(file, statement);
                    }
                    break;
                default:
                    break;
            }            

            return filePath;
        }

        public string CopyTemplate(Guid claimId, string template)
        {
            string docxFilePath = Path.Combine(root, "DOCX", string.Format("{0}{1}.docx", claimId, dateService.CurrentDateTimeString()));

            fileService.Copy(Path.Combine(root, "Templates", template), docxFilePath, true);

            return docxFilePath;
        }

        public void CreateDOCX(string filePath, XmlDocument xml, int sbi, string statementType)
        {
            FileInfo file = new FileInfo(filePath);

            file.IsReadOnly = false;

            using (WordprocessingDocument output = WordprocessingDocument.Open(filePath, true))
            {
                DocumentFormat.OpenXml.Wordprocessing.Body updatedBodyContent =
                    new DocumentFormat.OpenXml.Wordprocessing.Body(xml.DocumentElement.InnerXml);

                output.MainDocumentPart.Document.Body = updatedBodyContent;

                string content;

                using (StreamReader reader = new StreamReader(output.MainDocumentPart.FooterParts.First().GetStream()))
                {
                    content = reader.ReadToEnd();
                }

                Regex expression = null;

                switch(statementType)
                {
                    case "BPS":
                        expression = new Regex("csSBI");
                        break;
                    case "XB":
                        expression = new Regex("xxSBI");
                        break;
                    default:
                        break;
                }

                content = expression.Replace(content, sbi.ToString());

                using (StreamWriter writer = new StreamWriter(output.MainDocumentPart.FooterParts.First().GetStream(FileMode.Create)))
                {
                    writer.Write(content);
                }

                output.MainDocumentPart.Document.Save();
            }
        }

        public void Publish(string filePath)
        {
            fileService.Move(filePath, Path.Combine(outboundFolder, fileService.GetName(filePath)));
        }

        public XmlDocument TransformXML(string xml, string xslt)
        {
            StringWriter writer = new StringWriter();
            XmlDocument newXml = new XmlDocument();

            using (XmlWriter xmlWriter = XmlWriter.Create(writer))
            {
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(Path.Combine(root, "XSLT", xslt));
                transform.Transform(xml, xmlWriter);

                newXml.LoadXml(writer.ToString());
            }

            return newXml;
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
