using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.IO;
using System.Reflection;
using WebParserCore.Controllers;
using WebParserCore.Models;
using Paragraph = Spire.Doc.Documents.Paragraph;
using Section = Spire.Doc.Section;

namespace WebParserCore.FilePars
{
    public class DOCParser : FileParser
    {
        private string way = HomeController.mainPath;

        public DOCParser(string path, string name, Resposne resp): base(path, name, resp)
        {

        }

        public override bool Parse()
        {
            try
            {
                Document doc = new Document(path);
                string text = doc.GetText();
                tworker.WorkText(text);
                foreach (Section section in doc.Sections)
                {
                    //Get Each Paragraph of Section  
                    foreach (Paragraph paragraph in section.Paragraphs)
                    {
                        //Get Each Document Object of Paragraph Items  
                        foreach (DocumentObject docObject in paragraph.ChildObjects)
                        {
                            //If Type of Document Object is Picture, Extract.  
                            if (docObject.DocumentObjectType == DocumentObjectType.Picture)
                            {
                                DocPicture pic = docObject as DocPicture;
                                string fileName = (UniqueID.GetID('a') + UniqueID.GetID() + ".png").ToString();
                                if (!Directory.Exists(System.IO.Path.Combine(way, "img_for_txt"))) Directory.CreateDirectory(Path.Combine(way, "img_for_txt"));
                                string path = Path.Combine(way, "img_for_txt", "img_for_txt", fileName);

                                //Save Image  
                                pic.Image.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                                ImageBuffer.AddElement(new ImageInfo { Name = fileName, Path = path, CreationDate = DateTime.Now });
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}