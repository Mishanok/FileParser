using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore.FilePars
{
    public class FB2Parser : FileParser
    {
        private string way = HomeController.mainPath;

        public FB2Parser(string path, string name, Resposne resp) : base(path, name, resp) 
        {
           
        }

        public override bool Parse()
        {
            GetImages();
            XElement el = XElement.Load(path);
            string text = el.Value;
            tworker.WorkText(text);
            return true;
        }

        private void GetImages()
        {
            XmlNodeType type;
            string text = " ";
            XmlTextReader xtwpath = new XmlTextReader(path);
            while (xtwpath.Read())
            {
                type = xtwpath.NodeType;
                if (type == XmlNodeType.Element)
                {
                    if (xtwpath.Name == "binary")
                    {
                        xtwpath.Read();
                        text = xtwpath.Value;
                        SaveImage(text);
                    }
                }
            }
        }
        private void SaveImage(string base64)
        {
            if (string.IsNullOrEmpty(base64)) return;

            string fileName = (UniqueID.GetID('a') + UniqueID.GetID() + ".png").ToString();
            if(!Directory.Exists(Path.Combine(way, "img_for_txt"))) Directory.CreateDirectory(Path.Combine(way, "img_for_txt"));
            string path = Path.Combine(way, "img_for_txt", fileName);
            var buffer = Convert.FromBase64String(base64);
            using (var file = File.Create(path))
            {
                file.Write(buffer, 0, buffer.Length);
                file.Close();
            }
            ImageBuffer.AddElement(new ImageInfo { Name = fileName, Path = path, CreationDate = DateTime.Now});
        }
    }
}