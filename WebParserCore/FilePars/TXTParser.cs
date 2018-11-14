using System;
using System.Diagnostics;
using System.IO;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore.FilePars
{
    public class TXTParser : FileParser
    {
        private string way = HomeController.mainPath;

        public TXTParser(string path, string name, Resposne resp) : base(path, name, resp)
        {
        }

        public override bool Parse()
        {
            try
            {
                string sourceFileName = path;
                string destFileName = Path.Combine( way ,"original_txt", name);

                var text = File.ReadAllText(sourceFileName);
                TextChecker.Check(text);
                File.WriteAllText(destFileName, text);
                resp.TDesc = new TextDesc { Path = destFileName, Name = name, CreatinDate = DateTime.Now} ;
                JSONCreator.CreateJSON(new TextDesc { Path = destFileName, Name = name });
                return true;
            }
            catch(Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }
    }
}