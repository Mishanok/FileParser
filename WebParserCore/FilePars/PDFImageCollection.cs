using iTextSharp.text.pdf.parser;
using System;
using System.IO;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore.FilePars
{
    public class PDFImageCollection : IRenderListener
    {
        private string way = HomeController.mainPath;

        public void BeginTextBlock()
        {
            
        }

        public void EndTextBlock()
        {
            
        }

        public void RenderText(TextRenderInfo renderInfo)
        {
            return;
        }

        public void RenderImage(ImageRenderInfo renderInfo)
        {
            var imageObject = renderInfo.GetImage();

            var data = imageObject.GetImageAsBytes();

            string fileName = (UniqueID.GetID('a') + UniqueID.GetID() + "."+imageObject.GetFileType()).ToString();
            if (!Directory.Exists(System.IO.Path.Combine(way, "img_for_txt"))) Directory.CreateDirectory(System.IO.Path.Combine(way, "img_for_txt"));
            string path = System.IO.Path.Combine(way, "img_for_txt", fileName);
            File.WriteAllBytes(path,data);
            ImageBuffer.AddElement(new ImageInfo { Name = fileName, Path = path, CreationDate = DateTime.Now});
        }
    }
}