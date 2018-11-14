using System;
using System.Diagnostics;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using WebParserCore.Models;

namespace WebParserCore.FilePars
{
    class PDFParser : FileParser
    {
        public PDFParser(string path, string name, Resposne resp) : base(path, name, resp)
        {
          
        }

        public override bool Parse()
        {
            return GetText();
        }

        private bool GetText()
        {
            using (PdfReader reader = new PdfReader(path))
            {
                try
                {
                    StringBuilder text = new StringBuilder();
                    var imageCollection = new PDFImageCollection();
                    var pdfParser = new PdfReaderContentParser(reader);

                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        pdfParser.ProcessContent(i, imageCollection);
                    }

                    tworker.WorkText(text);
                    
                    return true;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.Message);
                    return false;
                }
            }
        }
    }
}
