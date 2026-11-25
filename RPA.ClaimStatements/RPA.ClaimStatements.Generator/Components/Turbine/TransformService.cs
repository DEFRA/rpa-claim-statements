using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Packaging;
using System.Xml.Xsl;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Models.Generation;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Context;
using Clippit;
using Clippit.Word;
using System.Xml.Linq;
using SkiaSharp;
using RPA.ClaimStatements.Generator.Services;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

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
                catch (Exception ex)
                {
                    // Keep the specific message (for example from the unsupported-image guard) instead of
                    // replacing it with a generic one.
                    if (ex is ClaimStatementGenerationException)
                    {
                        throw;
                    }
                    throw new ClaimStatementGenerationException(string.Format("Unable to convert statement to PDF - {0}", ex.Message), request.Claim.SUM.FRN, request.Claim.SUM.SchemeYear);
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
            
            RenderPdfAsync(filePath, pdfFilePath).GetAwaiter().GetResult();

            return pdfFilePath;
        }

        private static async Task RenderPdfAsync(string htmlFilePath, string pdfFilePath)
        {
            // Validate input before launching, without passing document content through arguments/logs.
            using (File.OpenRead(htmlFilePath)) { }
            string stagedPath = pdfFilePath + "." + Guid.NewGuid().ToString("N") + ".part";
            var startInfo = new ProcessStartInfo
            {
                FileName = OperatingSystem.IsWindows() ? "python" : "python3",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("-I");
            startInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "Rendering", "render_pdf.py"));
            startInfo.ArgumentList.Add(Path.GetFullPath(htmlFilePath));
            startInfo.ArgumentList.Add(Path.GetFullPath(stagedPath));
            using var process = new Process { StartInfo = startInfo };
            try
            {
                try
                {
                    process.Start();
                }
                catch (Win32Exception)
                {
                    throw new InvalidOperationException("PDF rendering requires Python and the WeasyPrint native dependencies. Use the supplied container image or install the documented prerequisites.");
                }

                Task<string> stdout = process.StandardOutput.ReadToEndAsync();
                Task<string> stderr = process.StandardError.ReadToEndAsync();
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                try
                {
                    await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (timeout.IsCancellationRequested)
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                    }
                    await process.WaitForExitAsync().ConfigureAwait(false);
                    await Task.WhenAll(stdout, stderr).ConfigureAwait(false);
                    throw new TimeoutException("PDF rendering exceeded the 60-second limit.");
                }
                await Task.WhenAll(stdout, stderr).ConfigureAwait(false);
                if (process.ExitCode == 3)
                {
                    throw new InvalidOperationException("Statement HTML contains an external resource or an image that could not be loaded.");
                }
                if (process.ExitCode != 0)
                {
                    // WeasyPrint diagnostics may contain document text, URLs or embedded image bytes.
                    throw new InvalidOperationException($"WeasyPrint PDF rendering failed (exit code {process.ExitCode}). Check the installed Python/native dependencies and document compatibility.");
                }
                if (!string.IsNullOrWhiteSpace(stdout.Result) || !string.IsNullOrWhiteSpace(stderr.Result))
                {
                    Console.Error.WriteLine("PDF renderer reported compatibility warnings; review template styling before acceptance.");
                }
                if (!File.Exists(stagedPath))
                {
                    throw new InvalidOperationException("PDF renderer did not produce a PDF document.");
                }
                using (var output = File.OpenRead(stagedPath))
                {
                    byte[] signature = new byte[5];
                    if (output.Length < 8 || output.Read(signature, 0, signature.Length) != signature.Length ||
                        System.Text.Encoding.ASCII.GetString(signature) != "%PDF-")
                    {
                        throw new InvalidOperationException("PDF renderer did not produce a PDF document.");
                    }
                    output.Seek(-8, SeekOrigin.End);
                    byte[] trailer = new byte[8];
                    output.ReadExactly(trailer);
                    if (!System.Text.Encoding.ASCII.GetString(trailer).TrimEnd().EndsWith("%%EOF", StringComparison.Ordinal))
                    {
                        throw new InvalidOperationException("PDF renderer produced an incomplete PDF document.");
                    }
                }
                File.Move(stagedPath, pdfFilePath, overwrite: true);
            }
            finally
            {
                File.Delete(stagedPath);
            }
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


                // Clippit silently drops embedded images that are not png, gif, tiff or jpeg before the
                // ImageHandler below runs, so fail the statement explicitly instead of producing an
                // incomplete document.
                EnsureNoUnsupportedEmbeddedImages(wDoc, frn, schemeYear);

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
                    // Clippit's SkiaSharp-based image handler (no GDI+ dependency). Clippit only passes
                    // png, gif, tiff and jpeg images to it; other formats such as bmp and wmf never reach
                    // this handler, which is why EnsureNoUnsupportedEmbeddedImages runs first.
                    ImageHandler = imageInfo =>
                    {
                        ++imageCounter;
                        string extension = imageInfo.ContentType.Split('/')[1].ToLower();
                        SKEncodedImageFormat? imageFormat = null;
                        if (extension == "png") imageFormat = SKEncodedImageFormat.Png;
                        else if (extension == "gif") imageFormat = SKEncodedImageFormat.Gif;
                        else if (extension == "jpeg") imageFormat = SKEncodedImageFormat.Jpeg;
                        else if (extension == "tiff")
                        {
                            // Preserve the pre-existing tiff->gif re-encode (browsers do not render
                            // tiff reliably); unlike the prior System.Drawing code, the embedded
                            // data-URI mime type below is now derived from the format actually
                            // encoded (gif), not the original tiff content type, which corrects a
                            // latent legacy mime/byte-content mismatch (browsers sniff image bytes,
                            // so this does not change observed rendering, only the declared mime
                            // type - documented here, not silently changed).
                            extension = "gif";
                            imageFormat = SKEncodedImageFormat.Gif;
                        }

                        if (imageFormat == null) return null;

                        string base64 = null;
                        // Quality 100 (max fidelity) is used for jpeg; png/gif ignore the
                        // parameter. This is a candidate default pending owner golden-baseline
                        // review, not a proven-identical match to the prior GDI+ default encoder.
                        using (SKData encoded = imageInfo.Image.Encode((SKEncodedImageFormat)imageFormat, 100))
                        {
                            if (encoded == null) return null;
                            base64 = System.Convert.ToBase64String(encoded.ToArray());
                        }

                        string mimeType = "image/" + extension;

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

        /// <summary>
        /// Pre-conversion guard against Clippit silently dropping images that are not png, gif, tiff or
        /// jpeg (for example bmp or wmf). Scans every <see cref="ImagePart"/> in the document body,
        /// headers and footers and throws a <see cref="ClaimStatementGenerationException"/> naming only
        /// the unsupported MIME type(s) and a count (no image content). The allow-list mirrors
        /// Clippit's own WmlToHtmlConverter.ImageContentTypes.
        /// </summary>
        private static readonly HashSet<string> SupportedHtmlConversionImageContentTypes =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "image/png",
                "image/gif",
                "image/tiff",
                "image/jpeg"
            };

        private static void EnsureNoUnsupportedEmbeddedImages(WordprocessingDocument wDoc, long frn, int schemeYear)
        {
            var unsupportedContentTypes = new List<string>();

            void CheckParts(IEnumerable<ImagePart> imageParts)
            {
                foreach (ImagePart imagePart in imageParts)
                {
                    if (!SupportedHtmlConversionImageContentTypes.Contains(imagePart.ContentType))
                    {
                        unsupportedContentTypes.Add(imagePart.ContentType);
                    }
                }
            }

            CheckParts(wDoc.MainDocumentPart.ImageParts);
            foreach (HeaderPart headerPart in wDoc.MainDocumentPart.HeaderParts)
            {
                CheckParts(headerPart.ImageParts);
            }
            foreach (FooterPart footerPart in wDoc.MainDocumentPart.FooterParts)
            {
                CheckParts(footerPart.ImageParts);
            }

            if (unsupportedContentTypes.Count > 0)
            {
                List<string> distinctFormats = unsupportedContentTypes.Distinct().OrderBy(x => x).ToList();
                throw new ClaimStatementGenerationException(
                    string.Format(
                        "Statement generation stopped: the claim template contains {0} embedded image(s) " +
                        "in format(s) [{1}] that the document-to-HTML converter does not support (only " +
                        "image/png, image/gif, image/tiff and image/jpeg are supported). These images " +
                        "would otherwise be silently omitted from the generated statement with no error, " +
                        "producing an incomplete document. This template must be re-saved with a " +
                        "supported image format, or the owner must accept/record the content loss before " +
                        "this guard is relaxed.",
                        unsupportedContentTypes.Count,
                        string.Join(", ", distinctFormats)),
                    frn,
                    schemeYear);
            }
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
                transform.Load(Path.Combine(AppContext.BaseDirectory, "XSLT", xslt));
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
