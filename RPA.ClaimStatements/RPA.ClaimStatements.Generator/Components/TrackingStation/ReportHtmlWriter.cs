using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RPA.ClaimStatements.Generator.Components.TrackingStation
{
    // Replaces System.Web.UI.HtmlTextWriter (not available on .NET 10) for the four tags the tracking report uses.
    // Output is byte-identical to the original: block tags break lines and indent with tabs, <p> stays inline.
    internal sealed class ReportHtmlWriter : System.IDisposable
    {
        internal enum Tag { Div, H2, H4, P }

        readonly TextWriter writer;
        readonly List<KeyValuePair<string, string>> styles = new List<KeyValuePair<string, string>>();
        readonly Stack<Tag> open = new Stack<Tag>();
        int indent;
        bool atLineStart = true;

        public ReportHtmlWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void AddStyleAttribute(string name, string value)
        {
            styles.Add(new KeyValuePair<string, string>(name, value));
        }

        public void RenderBeginTag(Tag tag)
        {
            WriteTabs();
            writer.Write("<" + Name(tag));

            if (styles.Count > 0)
            {
                writer.Write(" style=\"");
                foreach (var style in styles)
                {
                    writer.Write(style.Key + ":" + WebUtility.HtmlEncode(style.Value) + ";");
                }
                writer.Write("\"");
                styles.Clear();
            }

            writer.Write(">");
            open.Push(tag);

            if (IsBlock(tag))
            {
                writer.WriteLine();
                atLineStart = true;
                indent++;
            }
        }

        public void RenderEndTag()
        {
            var tag = open.Pop();

            if (IsBlock(tag))
            {
                writer.WriteLine();
                atLineStart = true;
                indent--;
                WriteTabs();
            }

            writer.Write("</" + Name(tag) + ">");
        }

        public void Write(string text)
        {
            WriteTabs();
            writer.Write(text);
        }

        public void Write(string format, params object[] args)
        {
            Write(string.Format(format, args));
        }

        public void Dispose()
        {
            writer.Flush();
        }

        void WriteTabs()
        {
            if (atLineStart)
            {
                writer.Write(new string('\t', indent));
                atLineStart = false;
            }
        }

        static bool IsBlock(Tag tag)
        {
            return tag != Tag.P;
        }

        static string Name(Tag tag)
        {
            return tag.ToString().ToLowerInvariant();
        }
    }
}
