using System;
using System.IO;
using System.Reflection;
using System.Text;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore
{
    public class TetxWorker
    {
        private string way = HomeController.mainPath;
        private Resposne resp;

        public TetxWorker( Resposne resp)
        {
            this.resp = resp;
        }

        public void WorkText()
        {
            string fileName = resp.Login + "_" + resp.Text.UniqeId + ".txt";
            if (!Directory.Exists(Path.Combine(way, "original_txt"))) Directory.CreateDirectory(Path.Combine(way, "original_txt"));
            File.WriteAllText(Path.Combine(way, "original_txt", fileName), resp.Text.Content);
            resp.TDesc = new TextDesc { Path = Path.Combine(way, "original_txt", fileName), Name = fileName, CreatinDate = DateTime.Now };
            JSONCreator.CreateJSON(resp.TDesc);
        }

        public void WorkText(StringBuilder text)
        {
            string fileName = resp.FileLoc.Name.Remove(resp.FileLoc.Name.Length - 4) + ".txt";
            string path = Path.Combine(way, "original_txt", fileName);
            var txt = TextChecker.Check(text);
            if (!Directory.Exists(Path.Combine(way, "original_txt"))) Directory.CreateDirectory(Path.Combine(way, "original_txt"));
            File.WriteAllText(path, txt);
            resp.TDesc = new TextDesc { Path = path, Name = fileName, CreatinDate = DateTime.Now };
            JSONCreator.CreateJSON(resp.TDesc);
        }

        internal void WorkText(string text)
        {
            string fileName = resp.FileLoc.Name.Remove(resp.FileLoc.Name.Length - 4) + ".txt";
            string path = Path.Combine(way, "original_txt", fileName);
            TextChecker.Check(text);
            if (!Directory.Exists(Path.Combine(way, "original_txt"))) Directory.CreateDirectory(Path.Combine(way, "original_txt"));
            File.WriteAllText(path, text);
            resp.TDesc = new TextDesc { Path = path, Name = fileName, CreatinDate = DateTime.Now };
            JSONCreator.CreateJSON(resp.TDesc);
        }
    }
}