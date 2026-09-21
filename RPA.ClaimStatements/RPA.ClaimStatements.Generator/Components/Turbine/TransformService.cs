using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Packaging;
using System.Text.RegularExpressions;
using System.Xml.Xsl;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Context;
using OpenXmlPowerTools;
using System.Xml.Linq;
using System.Drawing.Imaging;
using RPA.ClaimStatements.Generator.Services;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public class TransformService : ITransformService, IDisposable
    {
        IClaimStatementsContext db;
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
        object _DocSavelock = new object();

        // To prevent any multi-threading issues with the data context ensure there is only one instance.
        static readonly object _LogLock = new object();

        public TransformService(IClaimStatementsContext context, IFileService fileService, IDateService dateService, IFolderService folderService, 
            IConfigurationService configurationService, ILogService logService)
        {
            this.db = context;
            this.fileService = fileService;
            this.dateService = dateService;
            this.folderService = folderService;
            this.configurationService = configurationService;
            this.logService = logService;

            this.root = folderService.Root();
            this.transformations = context.Transformations.AsNoTracking().ToList();
            this.outboundFolder = folderService.GetPath("PDF Outbound");
            this.archiveFolder = folderService.GetPath("PDF Archive");
            this.archive = configurationService.IsActive("Archive");
        }

        public void Transform(Request request, ClaimStatement statement, string statementType)
        {
            try
            {
                string xmlFilePath = ConvertToXML(statement, statementType);

                Transformation transformation = transformations.Where(x => x.StatementType == statementType).FirstOrDefault();

                XmlDocument newXml = TransformXML(xmlFilePath, transformation.XSLT);

                string docxFilePath = null;
                string htmlFilePath = null;
                string pdfFilePath = null;

                try
                {
                    docxFilePath = CopyTemplate(request.Claim.ClaimID, transformation.Template);
                    htmlFilePath = CreateHtmlFromXmlDoc(docxFilePath, newXml, request.Claim.SUM.SBI, statementType, request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
                    pdfFilePath = ConvertToPDF(htmlFilePath, request.Claim.SUM.FRN, request.Claim.SUM.SBI, request.Claim.SUM.SchemeYear);
                }
                catch (Exception)
                {
                    throw new ClaimStatementGenerationException("Unable to convert statement to PDF", request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
                }

                fileService.Delete(docxFilePath);
                fileService.Delete(xmlFilePath);
                fileService.Delete(htmlFilePath);


                string archiveFilePath = "Not Archived";

                if (archive)
                {
                    try
                    {
                        archiveFilePath = fileService.Archive(pdfFilePath, archiveFolder);
                    }
                    catch (Exception ex)
                    {
                        throw new ClaimStatementGenerationException(string.Format("Unable to archive statement - {0}", ex.Message), request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
                    }
                }

                lock (_LogLock)
                {
                    logService.Log(request.Claim.SUM.FRN, request.Claim.SUM.SBI, request.Claim.SUM.SchemeYear, archiveFilePath, request.Claim.ClaimID);
                }

                try
                {
                    Publish(pdfFilePath);
                }
                catch (Exception ex)
                {
                    throw new ClaimStatementGenerationException(string.Format("Unable to publish statement - {0}", ex.Message), request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
                }
            }
            catch(Exception ex)
            {
                if (ex is ClaimStatementGenerationException)
                {
                    throw;
                }
                else
                {
                    throw new ClaimStatementGenerationException(string.Format("Failed to transform statement - {0}", ex.Message), request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
                }
            }
        }

        public string ConvertToPDF(string filePath, long frn, int sbi, int schemeYear)
        {
            string pdfFilePath = Path.Combine(root, "PDF", string.Format("RPA_OUT_BPS{0}_ClmStmt_{1}_{2}_{3}.pdf", schemeYear, frn, sbi, dateService.CurrentDateTimeString()));
            
            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            htmlToPdf.Zoom = 1.18F;

            var margin = new NReco.PdfGenerator.PageMargins();
            margin.Left = 12.7f;
            margin.Right = 25f;

            htmlToPdf.Margins = margin;
            htmlToPdf.Grayscale = true;
            htmlToPdf.GeneratePdfFromFile(filePath, null, pdfFilePath);

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

        public string CreateHtmlFromXmlDoc(string filePath, XmlDocument xml, int sbi, string statementType, long frn, int schemeYear)
        {
            string htmlFilePath = Path.Combine(root, "HTML", string.Format("RPA_OUT_BPS{0}_ClmStmt_{1}_{2}_{3}.html", schemeYear, frn, sbi, dateService.CurrentDateTimeString()));

            FileInfo file = new FileInfo(filePath);

            file.IsReadOnly = false;

            using (WordprocessingDocument wDoc = WordprocessingDocument.Open(filePath, true))
            {
                DocumentFormat.OpenXml.Wordprocessing.Body updatedBodyContent =
                    new DocumentFormat.OpenXml.Wordprocessing.Body(xml.DocumentElement.InnerXml);

                wDoc.MainDocumentPart.Document.Body = updatedBodyContent;

                lock (_DocSavelock)
                {
                    wDoc.MainDocumentPart.Document.Save();
                }

                int imageCounter = 0;
                var pageTitle = filePath;
                var part = wDoc.CoreFilePropertiesPart;
                if (part != null)
                    pageTitle = (string)part.GetXDocument()
                                            .Descendants(DC.title)
                                            .FirstOrDefault() ?? filePath;

                WmlToHtmlConverterSettings settings = new WmlToHtmlConverterSettings()
                {
                    AdditionalCss = "table {width: 700px;} tr td, table tr th { page-break-inside: avoid; }",
                    PageTitle = pageTitle,
                    FabricateCssClasses = true,
                    CssClassPrefix = "pt-",
                    RestrictToSupportedLanguages = false,
                    RestrictToSupportedNumberingFormats = false,
                    ImageHandler = imageInfo =>
                    {
                        ++imageCounter;
                        string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                        ImageFormat imageFormat = null;
                        if (extension == "png") imageFormat = ImageFormat.Png;
                        else if (extension == "gif") imageFormat = ImageFormat.Gif;
                        else if (extension == "bmp") imageFormat = ImageFormat.Bmp;
                        else if (extension == "jpeg") imageFormat = ImageFormat.Jpeg;
                        else if (extension == "tiff")
                        {
                            extension = "gif";
                            imageFormat = ImageFormat.Gif;
                        }
                        else if (extension == "x-wmf")
                        {
                            extension = "wmf";
                            imageFormat = ImageFormat.Wmf;
                        }

                        if (imageFormat == null) return null;

                        string base64 = null;
                        try
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                imageInfo.Bitmap.Save(ms, imageFormat);
                                var ba = ms.ToArray();
                                base64 = System.Convert.ToBase64String(ba);
                            }
                        }
                        catch (System.Runtime.InteropServices.ExternalException)
                        {
                            return null;
                        }

                        ImageFormat format = imageInfo.Bitmap.RawFormat;
                        ImageCodecInfo codec = ImageCodecInfo.GetImageDecoders()
                                                  .First(c => c.FormatID == format.Guid);
                        string mimeType = codec.MimeType;

                        string imageSource =
                               string.Format("data:{0};base64,{1}", mimeType, base64);

                        XElement img = new XElement(Xhtml.img,
                              new XAttribute(NoNamespace.src, imageSource),
                              imageInfo.ImgStyleAttribute,
                              imageInfo.AltText != null ?
                                   new XAttribute(NoNamespace.alt, imageInfo.AltText) : null);
                        return img;
                    }
                };

                XElement htmlElement = WmlToHtmlConverter.ConvertToHtml(wDoc, settings);
                var html = new XDocument(new XDocumentType("html", null, null, null),
                                                                            htmlElement);
                var htmlString = html.ToString(SaveOptions.DisableFormatting);

                File.WriteAllText(htmlFilePath, htmlString);
            }

            return htmlFilePath;
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
