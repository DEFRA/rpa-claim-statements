// These fixtures are synthetic and built at test time via DocumentFormat.OpenXml and SkiaSharp; they are not
// the real ClaimTemplate.docx or ClaimTemplateXB.docx (git-ignored) and make no claim of visual parity with
// them. They prove the DOCX merge and HTML conversion run end to end, with and without an embedded raster image.
using System;
using System.IO;
using System.Xml;
using Moq;
using NUnit.Framework;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Models.Exceptions;
using RPA.ClaimStatements.Generator.Tests.TestSupport;
using System.Collections.Generic;
using System.Linq;
using UglyToad.PdfPig;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace RPA.ClaimStatements.Generator.Tests.Components.Turbine
{
    [TestFixture]
    [Category("Turbine")]
    public class TransformServiceTests
    {
        Mock<IClaimStatementsContext> context;
        Mock<DbSet<Transformation>> transformationSet;
        Mock<IFileService> fileService;
        Mock<IDateService> dateService;
        Mock<IFolderService> folderService;
        Mock<IConfigurationService> configurationService;
        Mock<ILogService> logService;

        string testRoot;
        TransformService transformService;

        const string WNs = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
        const string RNs = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        const string WpNs = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing";
        const string ANs = "http://schemas.openxmlformats.org/drawingml/2006/main";
        const string PicNs = "http://schemas.openxmlformats.org/drawingml/2006/picture";

        [SetUp]
        public void Setup()
        {
            testRoot = Path.Combine(Path.GetTempPath(), "TransformServiceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(testRoot, "HTML"));
            Directory.CreateDirectory(Path.Combine(testRoot, "DOCX"));
            Directory.CreateDirectory(Path.Combine(testRoot, "PDF"));

            context = new Mock<IClaimStatementsContext>();
            transformationSet = new Mock<DbSet<Transformation>>().SetupData(new List<Transformation>());
            context.Setup(x => x.Transformations).Returns(transformationSet.Object);

            fileService = new Mock<IFileService>();
            dateService = new Mock<IDateService>();
            dateService.Setup(x => x.CurrentDateTimeString()).Returns("20261022T000000");
            folderService = new Mock<IFolderService>();
            folderService.Setup(x => x.Root()).Returns(testRoot);
            folderService.Setup(x => x.GetPath(It.IsAny<string>())).Returns(testRoot);
            configurationService = new Mock<IConfigurationService>();
            configurationService.Setup(x => x.IsActive(It.IsAny<string>())).Returns(false);
            logService = new Mock<ILogService>();

            transformService = new TransformService(
                context.Object, fileService.Object, dateService.Object, folderService.Object,
                configurationService.Object, logService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (Directory.Exists(testRoot)) Directory.Delete(testRoot, true);
            }
            catch (IOException)
            {
                // best-effort temp cleanup only; never let cleanup failures fail the test result.
            }
        }

        [TestCase("ClaimXSLT.xslt")]
        [TestCase("ClaimXSLTXB.xslt")]
        public void TransformXML_LoadsTheXsltShippedWithTheApplication_NotFromTheWorkingRoot(string xslt)
        {
            string xmlPath = Path.Combine(testRoot, "claim.xml");
            File.WriteAllText(xmlPath, "<ClaimStatement />");
            Assert.That(Directory.Exists(Path.Combine(testRoot, "XSLT")), Is.False);

            XmlDocument result = transformService.TransformXML(xmlPath, xslt);

            Assert.That(result.DocumentElement.LocalName, Is.EqualTo("document"));
            Assert.That(result.DocumentElement.NamespaceURI, Is.EqualTo(WNs));
        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_RendersSearchableA4Pages_WithExistingNameAndGrayscale()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "synthetic statement.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                    body { font-family: Arial; color: red; }
                    .next { break-before: page; }
                    @media print { .screen { display: none; } }
                    </style></head><body>
                    <h1>Statement 2023</h1><p class='screen'>Screen content</p>
                    <table><tr><td>Payment</td><td>&pound;123.45</td></tr></table>
                    <div class='next'>Second page 678.90</div></body></html>");

            string pdfPath = transformService.ConvertToPDF(htmlPath, 1234567890, 123456789, 2023);

            Assert.That(Path.GetFileName(pdfPath), Is.EqualTo("RPA_OUT_BPS2023_ClmStmt_1234567890_123456789_20261022T000000.pdf"));
            using (var pdf = PdfDocument.Open(pdfPath))
            {
                Assert.That(pdf.NumberOfPages, Is.EqualTo(2));
                Assert.That(pdf.GetPage(1).Text, Does.Contain("Statement 2023").And.Contain("\u00a3123.45").And.Contain("Screen content"));
                Assert.That(pdf.GetPage(2).Text, Does.Contain("Second page 678.90"));
                Assert.That(pdf.GetPage(1).Width, Is.EqualTo(595.28).Within(1));
                Assert.That(pdf.GetPage(1).Height, Is.EqualTo(841.89).Within(1));
                foreach (var letter in pdf.GetPage(1).Letters)
                {
                    var colour = letter.Color.ToRGBValues();
                    Assert.That(colour.r, Is.EqualTo(colour.g).Within(0.01));
                    Assert.That(colour.g, Is.EqualTo(colour.b).Within(0.01));
                }
            }
        }

        [TestCase("<img src='https://synthetic.invalid/secret.png'>")]
        [TestCase("<img src='file:///not-permitted.png'>")]
        [TestCase("<img src='data:image/png;base64,AAAA'>")]
        [TestCase("<link rel='stylesheet' href='https://synthetic.invalid/style.css'>")]
        [TestCase("<link rel='stylesheet' href='relative.css'>")]
        [TestCase("<style>body { background-image: url(file:///not-permitted.png); }</style>")]
        [TestCase("<img>")]
        [TestCase("<img src='data:image/svg+xml,%3Csvg%3E%3C/svg%3E'>")]
        [RequiresWeasyPrint]
        public void ConvertToPDF_ResourceFailure_DoesNotPublishPdf(string image)
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "broken.html");
            File.WriteAllText(htmlPath, "<html><body>" + image + "</body></html>");
            var error = Assert.Throws<InvalidOperationException>(() => transformService.ConvertToPDF(htmlPath, 1, 2, 2023));
            Assert.That(error.Message, Does.Contain("resource").Or.Contain("image"));
            Assert.That(Directory.GetFiles(Path.Combine(testRoot, "PDF")), Is.Empty);
        }

        [Test]
        public void ConvertToPDF_MissingHtml_ThrowsWithoutProducingPdf()
        {
            Assert.Throws<FileNotFoundException>(() =>
                transformService.ConvertToPDF(Path.Combine(testRoot, "missing.html"), 1, 2, 2023));
            Assert.That(Directory.GetFiles(Path.Combine(testRoot, "PDF")), Is.Empty);
        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_DisablesDocumentScripts()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "script.html");
            File.WriteAllText(htmlPath, "<html><body>Original statement<script>document.body.innerText='Replaced statement';</script></body></html>");
            string pdfPath = transformService.ConvertToPDF(htmlPath, 1, 2, 2023);
            using (var pdf = PdfDocument.Open(pdfPath))
            {
                Assert.That(pdf.GetPage(1).Text, Does.Contain("Original statement").And.Not.Contain("Replaced statement"));
            }

        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_PreservesPhysicalMarginsAndScaledText_WithCssGrayscale()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "scaled.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                @page { size: A3 !important; margin: 0 !important; }
                body { margin: 0; font: 12pt 'Liberation Sans'; --ink: #0080ff; color: var(--ink); }
                p { margin: 0; } .right { position: absolute; top: 0; right: 0; color: hsl(120,100%,25%); }
                @media screen { .nested { color: rebeccapurple; } }
                </style></head><body><p>L</p><p class='right'>R</p>
                <p class='nested'>Nested</p><p style='color:rgba(255,0,0,0.8)'>Inline</p></body></html>");

            using (var pdf = PdfDocument.Open(transformService.ConvertToPDF(htmlPath, 1, 2, 2023)))
            {
                var page = pdf.GetPage(1);
                Assert.That(page.Width, Is.EqualTo(595.28).Within(0.1));
                Assert.That(page.Height, Is.EqualTo(841.89).Within(0.1));
                var left = page.Letters.Single(letter => letter.Value == "L");
                var right = page.Letters.Single(letter => letter.Value == "R");
                Assert.That(left.PointSize, Is.EqualTo(12 * 1.18).Within(0.1));
                Assert.That(left.BoundingBox.Left, Is.EqualTo(12.7 * 72 / 25.4).Within(2));
                Assert.That(right.BoundingBox.Right, Is.EqualTo(page.Width - 25 * 72 / 25.4).Within(2));
                Assert.That(page.Text, Does.Contain("Nested").And.Contain("Inline"));
                foreach (var letter in page.Letters)
                {
                    var colour = letter.Color.ToRGBValues();
                    Assert.That(colour.r, Is.EqualTo(colour.g).Within(0.01));
                    Assert.That(colour.g, Is.EqualTo(colour.b).Within(0.01));
                }
            }
        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_WideContent_ShrinksToFitInsteadOfClipping()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "wide.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                body { margin: 0; font: 12pt 'Liberation Sans'; } table { width: 700px; border-collapse: collapse; }
                </style></head><body><table><tr><td>Left edge</td><td style='text-align:right'>Right edge 1,040.00</td></tr></table>
                <p>Narrow body text</p></body></html>");

            using (var pdf = PdfDocument.Open(transformService.ConvertToPDF(htmlPath, 1, 2, 2023)))
            {
                var page = pdf.GetPage(1);
                double rightLimit = page.Width - 25 * 72 / 25.4;
                Assert.That(page.Text, Does.Contain("Right edge 1,040.00"));
                Assert.That(page.Letters.Max(letter => letter.BoundingBox.Right), Is.LessThanOrEqualTo(rightLimit + 1));
                Assert.That(page.Letters.First(letter => letter.Value == "L").PointSize, Is.LessThan(12 * 1.18 - 0.5));
            }
        }

        [Test]
        [NonParallelizable]
        [RequiresWeasyPrint]
        public void ConvertToPDF_MapsWordSystemColoursAndSymbolBullets()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "word.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                td { border: solid windowtext 1.0pt; font-family: 'Liberation Sans'; }
                span.bullet { font-family: Symbol; }
                </style></head><body><table style='border-collapse: collapse'><tr><td>Cell</td></tr></table>
                <p><span class='bullet'>&#xF0B7;</span> Item</p></body></html>");

            var originalError = Console.Error;
            var error = new StringWriter();
            try
            {
                Console.SetError(error);
                using (var pdf = PdfDocument.Open(transformService.ConvertToPDF(htmlPath, 1, 2, 2023)))
                {
                    Assert.That(pdf.GetPage(1).Text, Does.Contain("\u2022").And.Contain("Item"));
                }
            }
            finally
            {
                Console.SetError(originalError);
            }
            Assert.That(error.ToString(), Does.Not.Contain("compatibility warnings"), "windowtext must not be rejected as an invalid colour.");
        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_TrailingBlankParagraphs_DoNotCreateBlankPage()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "trailing.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                body, p { margin: 0; font: 11pt 'Liberation Sans'; }
                </style></head><body><div><div style='height: 880px'>Full page statement</div>
                <p>&nbsp;</p><p><span>&nbsp;</span></p></div></body></html>");

            using (var pdf = PdfDocument.Open(transformService.ConvertToPDF(htmlPath, 1, 2, 2023)))
            {
                Assert.That(pdf.NumberOfPages, Is.EqualTo(1));
            }
        }

        [Test]
        [RequiresWeasyPrint]
        public void ConvertToPDF_TabStopLabels_AlignValuesOnTheSameColumn()
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "tabs.html");
            File.WriteAllText(htmlPath, @"<!doctype html><html><head><style>
                p { margin: 0; font: 11pt 'Liberation Sans'; } body { margin: 0; }
                span.label { display: inline-block; min-width: 0.500in; text-indent: 0; }
                span.tab { margin: 0 0 0 0.50in; padding: 0; }
                </style></head><body>
                <p><span class='label'>Email:</span><span><span class='tab'>&#160;</span></span><span>shortvalue</span></p>
                <p><span class='label'>Address:</span><span><span class='tab'>&#160;</span></span><span>longvalue</span></p>
                <p><span class='label'></span><span><span class='tab'>&#160;</span></span><span>continuation</span></p>
                </body></html>");

            using (var pdf = PdfDocument.Open(transformService.ConvertToPDF(htmlPath, 1, 2, 2023)))
            {
                var words = pdf.GetPage(1).GetWords().ToList();
                double labelLeft = words.Single(word => word.Text == "Email:").BoundingBox.Left;
                double shortLeft = words.Single(word => word.Text == "shortvalue").BoundingBox.Left;
                double longLeft = words.Single(word => word.Text == "longvalue").BoundingBox.Left;
                double continuationLeft = words.Single(word => word.Text == "continuation").BoundingBox.Left;
                // 0.5in label minimum plus the 0.5in tab, scaled by 1.18, plus the tab's non-breaking space.
                Assert.That(shortLeft - labelLeft, Is.EqualTo(96 * 1.18 * 0.75 + 3.6).Within(2.5));
                Assert.That(longLeft, Is.EqualTo(shortLeft).Within(0.5), "A label wider than its tab stop must not push the value right.");
                Assert.That(continuationLeft, Is.EqualTo(shortLeft).Within(0.5));
            }
        }

        [TestCase("exit")]
        [TestCase("missing")]
        [TestCase("invalid")]
        [TestCase("truncated")]
        [TestCase("timeout")]
        [Platform("Linux")]
        [UnsupportedOSPlatform("windows")]
        [NonParallelizable]
        public void ConvertToPDF_ProcessFailure_ThrowsAndCleansStagedOutput(string failure)
        {
            string htmlPath = Path.Combine(testRoot, "HTML", "process.html");
            File.WriteAllText(htmlPath, "<p>Synthetic statement</p>");
            string bin = Path.Combine(testRoot, "bin");
            Directory.CreateDirectory(bin);
            string interpreter = Path.Combine(bin, "python3");
            string command = failure switch
            {
                "exit" => "exit 7",
                "missing" => "exit 0",
                "invalid" => "printf 'invalid output' > \"$4\"",
                "truncated" => "printf '%s' '%PDF-1.7 truncated' > \"$4\"",
                _ => "/bin/sleep 120 &\nwait"
            };
            File.WriteAllText(interpreter, "#!/bin/sh\n" + command + "\n");
            File.SetUnixFileMode(interpreter, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            string originalPath = Environment.GetEnvironmentVariable("PATH");
            var elapsed = Stopwatch.StartNew();
            try
            {
                Environment.SetEnvironmentVariable("PATH", bin + Path.PathSeparator + originalPath);
                if (failure == "timeout")
                {
                    Assert.Throws<TimeoutException>(() => transformService.ConvertToPDF(htmlPath, 1, 2, 2023));
                    Assert.That(elapsed.Elapsed.TotalSeconds, Is.InRange(59, 75));
                }
                else
                {
                    Assert.Throws<InvalidOperationException>(() => transformService.ConvertToPDF(htmlPath, 1, 2, 2023));
                }
                Assert.That(Directory.GetFiles(Path.Combine(testRoot, "PDF")), Is.Empty);
            }
            finally
            {
                Environment.SetEnvironmentVariable("PATH", originalPath);
            }
        }

        // Builds a minimal, valid, unencrypted WordprocessingDocument (plain OPC package) at
        // docxPath - a safe synthetic stand-in for the real, git-ignored
        // ClaimTemplate*.docx files. Returns the
        // MainDocumentPart image relationship id if a PNG image part was embedded, else null.
        static string CreateSyntheticDocx(string docxPath, bool withImage, PartTypeInfo? imagePartType = null, byte[] imageBytes = null)
        {
            string relId = null;

            using (WordprocessingDocument doc = WordprocessingDocument.Create(docxPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body(new Paragraph(new Run(new Text("placeholder - replaced by CreateHtmlFromXmlDoc")))));

                // Clippit's FormattingAssembler.AssembleFormatting (invoked internally by
                // WmlToHtmlConverter.ConvertToHtml) unconditionally reads
                // MainDocumentPart.StyleDefinitionsPart and .DocumentSettingsPart via GetXDocument()
                // - both parts must exist (even with minimal/empty content) or Clippit throws
                // ArgumentNullException. A real Word-authored .docx always includes these parts;
                // this minimal synthetic fixture must add them explicitly to be structurally valid.
                StyleDefinitionsPart stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();
                stylesPart.Styles.Save();

                DocumentSettingsPart settingsPart = mainPart.AddNewPart<DocumentSettingsPart>();
                settingsPart.Settings = new Settings();
                settingsPart.Settings.Save();

                if (withImage)
                {
                    PartTypeInfo effectivePartType = imagePartType ?? ImagePartType.Png;
                    byte[] effectiveBytes = imageBytes ?? CreateSyntheticPng();
                    ImagePart imagePart = mainPart.AddImagePart(effectivePartType);
                    using (MemoryStream imageStream = new MemoryStream(effectiveBytes))
                    {
                        imagePart.FeedData(imageStream);
                    }
                    relId = mainPart.GetIdOfPart(imagePart);
                }

                mainPart.Document.Save();
            }

            return relId;
        }

        // A tiny, deterministic, solid-colour 4x4 PNG built via SkiaSharp (the same cross-platform
        // image stack this batch migrated TransformService's ImageHandler onto) - not derived from
        // any licensed/production asset.
        static byte[] CreateSyntheticPng()
        {
            using (SKBitmap bitmap = new SKBitmap(4, 4))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(new SKColor(0, 102, 51));
                }
                using (SKData encoded = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return encoded.ToArray();
                }
            }
        }

        // A tiny, deterministic, solid-colour 4x4 JPEG built via SkiaSharp - used to prove the
        // ImageHandler's "jpeg" branch (quality-100 encode path) also genuinely round-trips, not
        // just the "png" branch already covered above. Every SkiaSharp handle (SKBitmap, SKCanvas,
        // SKData) is explicitly disposed via "using" in the same pattern as CreateSyntheticPng -
        // SkiaSharp native handles are unmanaged-backed and must not be left to the GC.
        static byte[] CreateSyntheticJpeg()
        {
            using (SKBitmap bitmap = new SKBitmap(4, 4))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    canvas.Clear(new SKColor(153, 51, 0));
                }
                using (SKData encoded = bitmap.Encode(SKEncodedImageFormat.Jpeg, 100))
                {
                    return encoded.ToArray();
                }
            }
        }

        // A minimal, hand-built 1x1 24bpp Windows BMP. BMP is a format Clippit's WmlToHtmlConverter silently drops, so
        // this exercises the unsupported-embedded-image guard (TransformService.EnsureNoUnsupportedEmbeddedImages).
        // It is built by hand because SkiaSharp cannot encode BMP here; the guard only inspects the ImagePart content
        // type, never the pixel bytes, and rejects the fixture before conversion runs.
        static byte[] CreateSyntheticBmp()
        {
            // BITMAPFILEHEADER (14 bytes) + BITMAPINFOHEADER (40 bytes) + 1 BGR pixel padded to a
            // 4-byte row boundary (4 bytes) = 58 bytes total.
            const int fileHeaderSize = 14;
            const int infoHeaderSize = 40;
            const int rowSize = 4; // ((24 * 1 + 31) / 32) * 4
            const int pixelDataSize = rowSize * 1;
            const int pixelDataOffset = fileHeaderSize + infoHeaderSize;
            const int fileSize = pixelDataOffset + pixelDataSize;

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // BITMAPFILEHEADER
                writer.Write((byte)'B');
                writer.Write((byte)'M');
                writer.Write(fileSize);
                writer.Write(0); // reserved
                writer.Write(pixelDataOffset);

                // BITMAPINFOHEADER
                writer.Write(infoHeaderSize);
                writer.Write(1); // width
                writer.Write(1); // height
                writer.Write((short)1); // planes
                writer.Write((short)24); // bits per pixel
                writer.Write(0); // compression (BI_RGB)
                writer.Write(pixelDataSize);
                writer.Write(0); // x pixels per metre
                writer.Write(0); // y pixels per metre
                writer.Write(0); // colours used
                writer.Write(0); // important colours

                // Pixel data: one BGR pixel, padded to the 4-byte row boundary.
                writer.Write(new byte[] { 153, 51, 51, 0 });

                writer.Flush();
                return stream.ToArray();
            }
        }

        // DocumentFormat.OpenXml.OpenXmlElement(string outerXml) (the constructor behind
        // `new Body(xml.DocumentElement.InnerXml)` in CreateHtmlFromXmlDoc/CreateDOCX) requires its
        // argument to be the single, complete outer XML of an element whose own namespace/local
        // name matches the target type (confirmed via DocumentFormat.OpenXml.Framework's
        // OpenXmlElement.ValidOuterXml check on the resolved 3.5.1 package source) - i.e. a single
        // literal "<w:body>...</w:body>" element, not a bare collection of sibling content nodes.
        // The real XSLT-transform output (TransformXML) must therefore itself produce a wrapper
        // root element whose single child is a complete "<w:body>" element; this fixture mirrors
        // that exact shape so `xml.DocumentElement.InnerXml` yields that one "<w:body>...</w:body>"
        // string, matching real production usage.
        static XmlDocument WrapBodyContentXml(string bodyContentFragment)
        {
            string wrapped = string.Format(
                "<root><w:body xmlns:w=\"{0}\" xmlns:r=\"{1}\" xmlns:wp=\"{2}\" xmlns:a=\"{3}\" xmlns:pic=\"{4}\">{5}</w:body></root>",
                WNs, RNs, WpNs, ANs, PicNs, bodyContentFragment);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(wrapped);
            return xml;
        }

        [Test]
        public void Test_CreateHtmlFromXmlDoc_NoImage_ProducesHtmlWithPlainText()
        {
            string docxPath = Path.Combine(testRoot, "DOCX", "synthetic_no_image.docx");
            CreateSyntheticDocx(docxPath, withImage: false);

            // en-GB currency formatting + a financial-looking total, deliberately synthetic
            // (not real claimant data).
            XmlDocument bodyXml = WrapBodyContentXml(
                "<w:p><w:r><w:t>Synthetic claim statement total: \u00a3123.45 (en-GB)</w:t></w:r></w:p>");

            string htmlFilePath = transformService.CreateHtmlFromXmlDoc(docxPath, bodyXml, sbi: 123456789, statementType: "BPS", frn: 1000000001, schemeYear: 2026);

            Assert.That(File.Exists(htmlFilePath), Is.True, "CreateHtmlFromXmlDoc must produce a real HTML file on disk.");

            string html = File.ReadAllText(htmlFilePath);

            Assert.That(html, Does.Contain("Synthetic claim statement total"));
            Assert.That(html, Does.Contain("\u00a3123.45"));
            Assert.That(html, Does.Not.Contain("data:image/"), "A body with no drawing/image markup must not produce an <img> data-URI.");
        }

        [Test]
        [RequiresWeasyPrint]
        public void Test_CreateHtmlFromXmlDoc_WithPngImage_ProducesBase64ImgTag()
        {
            string docxPath = Path.Combine(testRoot, "DOCX", "synthetic_with_image.docx");
            string relId = CreateSyntheticDocx(docxPath, withImage: true);

            string drawingFragment = string.Format(
                "<w:p><w:r><w:drawing>" +
                "<wp:inline><wp:extent cx=\"914400\" cy=\"914400\"/>" +
                "<wp:docPr id=\"1\" name=\"Synthetic test image\" descr=\"Synthetic test image\"/>" +
                "<a:graphic><a:graphicData uri=\"{0}\">" +
                "<pic:pic><pic:blipFill><a:blip r:embed=\"{1}\"/></pic:blipFill></pic:pic>" +
                "</a:graphicData></a:graphic>" +
                "</wp:inline></w:drawing></w:r></w:p>",
                PicNs, relId);

            XmlDocument bodyXml = WrapBodyContentXml(
                "<w:p><w:r><w:t>Synthetic claim statement with embedded image (en-GB)</w:t></w:r></w:p>" + drawingFragment);

            string htmlFilePath = transformService.CreateHtmlFromXmlDoc(docxPath, bodyXml, sbi: 123456789, statementType: "BPS", frn: 1000000002, schemeYear: 2026);

            Assert.That(File.Exists(htmlFilePath), Is.True, "CreateHtmlFromXmlDoc must produce a real HTML file on disk.");

            string html = File.ReadAllText(htmlFilePath);

            Assert.That(html, Does.Contain("Synthetic claim statement with embedded image"));
            Assert.That(html, Does.Contain("<img"), "An embedded PNG drawing must round-trip through Clippit's WmlToHtmlConverter + the SkiaSharp ImageHandler into a real <img> element.");
            Assert.That(html, Does.Contain("data:image/png;base64,"), "The PNG image must be re-encoded via SkiaSharp (SKBitmap.Encode) and base64-embedded, proving the System.Drawing -> SkiaSharp ImageHandler swap genuinely works end-to-end (not a stub).");

            string pdfPath = transformService.ConvertToPDF(htmlFilePath, 1000000002, 123456789, 2026);
            using (var pdf = PdfDocument.Open(pdfPath))
            {
                Assert.That(pdf.NumberOfPages, Is.EqualTo(1));
                Assert.That(pdf.GetPage(1).Text, Does.Contain("Synthetic claim statement with embedded image"));
                Assert.That(pdf.GetPage(1).GetImages().Any(), Is.True);
                foreach (var image in pdf.GetPage(1).GetImages())
                {
                    Assert.That(image.TryGetPng(out byte[] png), Is.True);
                    using (var bitmap = SKBitmap.Decode(png))
                    {
                        Assert.That(bitmap, Is.Not.Null);
                        var colour = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);
                        Assert.That(colour.Red, Is.EqualTo(colour.Green).Within(1));
                        Assert.That(colour.Green, Is.EqualTo(colour.Blue).Within(1));
                    }
                }
            }
        }

        [Test]
        public void Test_CreateHtmlFromXmlDoc_WithJpegImage_ProducesBase64ImgTag()
        {
            // Second already-supported format (distinct from the PNG test above), with explicit
            // attention to SkiaSharp resource disposal (SKBitmap/SKCanvas/SKData all created and
            // disposed via "using" in CreateSyntheticJpeg - see that helper).
            string docxPath = Path.Combine(testRoot, "DOCX", "synthetic_with_jpeg_image.docx");
            string relId = CreateSyntheticDocx(docxPath, withImage: true, imagePartType: ImagePartType.Jpeg, imageBytes: CreateSyntheticJpeg());

            string drawingFragment = string.Format(
                "<w:p><w:r><w:drawing>" +
                "<wp:inline><wp:extent cx=\"914400\" cy=\"914400\"/>" +
                "<wp:docPr id=\"1\" name=\"Synthetic test image\" descr=\"Synthetic test image\"/>" +
                "<a:graphic><a:graphicData uri=\"{0}\">" +
                "<pic:pic><pic:blipFill><a:blip r:embed=\"{1}\"/></pic:blipFill></pic:pic>" +
                "</a:graphicData></a:graphic>" +
                "</wp:inline></w:drawing></w:r></w:p>",
                PicNs, relId);

            XmlDocument bodyXml = WrapBodyContentXml(
                "<w:p><w:r><w:t>Synthetic claim statement with embedded JPEG image (en-GB)</w:t></w:r></w:p>" + drawingFragment);

            string htmlFilePath = transformService.CreateHtmlFromXmlDoc(docxPath, bodyXml, sbi: 123456789, statementType: "BPS", frn: 1000000003, schemeYear: 2026);

            Assert.That(File.Exists(htmlFilePath), Is.True, "CreateHtmlFromXmlDoc must produce a real HTML file on disk.");

            string html = File.ReadAllText(htmlFilePath);

            Assert.That(html, Does.Contain("Synthetic claim statement with embedded JPEG image"));
            Assert.That(html, Does.Contain("<img"), "An embedded JPEG drawing must round-trip through Clippit's WmlToHtmlConverter + the SkiaSharp ImageHandler into a real <img> element.");
            Assert.That(html, Does.Contain("data:image/jpeg;base64,"), "The JPEG image must be re-encoded via SkiaSharp (SKBitmap.Encode, SKEncodedImageFormat.Jpeg) and base64-embedded, proving the second supported-format branch genuinely works end-to-end (not just PNG).");
        }

        [Test]
        public void Test_CreateHtmlFromXmlDoc_WithBmpImage_ThrowsClearUnsupportedFormatError()
        {
            // BMP is NOT in Clippit's own ImageContentTypes allow-list (image/png, image/gif,
            // image/tiff, image/jpeg only - see Clippit/Word/WmlToHtmlConverter.cs,
            // ImageContentTypes/ProcessDrawing). Without the EnsureNoUnsupportedEmbeddedImages
            // guard, Clippit would silently produce HTML with NO <img> element for this image -
            // an incomplete statement that reports success. This test proves the guard turns that
            // silent content loss into a loud, explicit, non-PII failure instead.
            string docxPath = Path.Combine(testRoot, "DOCX", "synthetic_with_bmp_image.docx");
            string relId = CreateSyntheticDocx(docxPath, withImage: true, imagePartType: ImagePartType.Bmp, imageBytes: CreateSyntheticBmp());

            string drawingFragment = string.Format(
                "<w:p><w:r><w:drawing>" +
                "<wp:inline><wp:extent cx=\"914400\" cy=\"914400\"/>" +
                "<wp:docPr id=\"1\" name=\"Synthetic test image\" descr=\"Synthetic test image\"/>" +
                "<a:graphic><a:graphicData uri=\"{0}\">" +
                "<pic:pic><pic:blipFill><a:blip r:embed=\"{1}\"/></pic:blipFill></pic:pic>" +
                "</a:graphicData></a:graphic>" +
                "</wp:inline></w:drawing></w:r></w:p>",
                PicNs, relId);

            XmlDocument bodyXml = WrapBodyContentXml(
                "<w:p><w:r><w:t>Synthetic claim statement with embedded BMP image (en-GB)</w:t></w:r></w:p>" + drawingFragment);

            string htmlFilePath = Path.Combine(testRoot, "HTML", string.Format("RPA_OUT_BPS{0}_ClmStmt_{1}_{2}_{3}.html", 2026, 1000000004, 123456789, "20261022T000000"));

            ClaimStatementGenerationException thrown = Assert.Throws<ClaimStatementGenerationException>(() =>
                transformService.CreateHtmlFromXmlDoc(docxPath, bodyXml, sbi: 123456789, statementType: "BPS", frn: 1000000004, schemeYear: 2026),
                "An embedded BMP image must cause CreateHtmlFromXmlDoc to throw a clear, explicit error instead of silently producing an incomplete statement.");

            Assert.That(thrown.Message, Does.Contain("image/bmp"), "The thrown exception must name the specific unsupported format so the cause is immediately clear (not a generic message).");
            Assert.That(thrown.Message, Does.Not.Contain("base64"), "The guard must never embed raw image bytes/content in the exception message.");
            Assert.That(thrown.FRN, Is.EqualTo(1000000004));
            Assert.That(thrown.SchemeYear, Is.EqualTo(2026));
            Assert.That(File.Exists(htmlFilePath), Is.False, "The guard must run BEFORE any HTML is written, so no partial/incomplete output file is ever produced on disk for the rejected document.");
        }
    }
}
