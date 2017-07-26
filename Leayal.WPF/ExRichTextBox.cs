using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Leayal.WPF
{
    public class ExRichTextBox : RichTextBox
    {
        public ExRichTextBox(FlowDocument flowDocument) : base(flowDocument)
        {

        }

        public ExRichTextBox() : base()
        {

        }

        internal static readonly Thickness emptyThickness = new Thickness(0, 0, 0, 0);

        public void AppendText(Run text, bool newline, Thickness thickness)
        {
            Paragraph p = null;
            if (newline)
            {
                p = new Paragraph() { Margin = thickness };
                p.Inlines.Add(text);
                this.Document.Blocks.Add(p);
            }
            else
            {
                p = this.Document.Blocks.LastBlock as Paragraph;
                if (p == null)
                    p = new Paragraph() { Margin = thickness };
                p.Inlines.Add(text);
            }
        }

        public void AppendText(IEnumerable<Run> texts, bool newline)
        {
            this.AppendText(texts, newline, emptyThickness);
        }

        public void AppendText(IEnumerable<Run> texts, bool newline, Thickness thickness)
        {
            Paragraph p = null;
            if (newline)
            {
                p = new Paragraph() { Margin = thickness };
                foreach (Run _run in texts)
                    p.Inlines.Add(_run);
                this.Document.Blocks.Add(p);
            }
            else
            {
                p = this.Document.Blocks.LastBlock as Paragraph;
                if (p == null)
                    p = new Paragraph() { Margin = thickness };
                foreach (Run _run in texts)
                    p.Inlines.Add(_run);
            }
        }

        public void AppendText(Run[] texts, bool newline)
        {
            this.AppendText(texts, newline, emptyThickness);
        }

        public void AppendText(Run[] texts, bool newline, Thickness thickness)
        {

            Paragraph p = null;
            if (newline)
            {
                p = new Paragraph() { Margin = thickness };
                for (int i = 0; i < texts.Length; i++)
                    p.Inlines.Add(texts[i]);
                this.Document.Blocks.Add(p);
            }
            else
            {
                p = this.Document.Blocks.LastBlock as Paragraph;
                if (p == null)
                    p = new Paragraph() { Margin = thickness };
                for (int i = 0; i < texts.Length; i++)
                    p.Inlines.Add(texts[i]);
            }
        }

        public void AppendText(string text, bool newline)
        {
            this.AppendText(text, newline, emptyThickness);
        }

        public void AppendText(string text, bool newline, Thickness thickness)
        {
            Paragraph p = null;
            if (newline)
            {
                p = new Paragraph() { Margin = thickness };
                p.Inlines.Add(text);
                this.Document.Blocks.Add(p);
            }
            else
            {
                p = this.Document.Blocks.LastBlock as Paragraph;
                if (p == null)
                    p = new Paragraph() { Margin = thickness };
                p.Inlines.Add(text);
            }
        }
    }
}
