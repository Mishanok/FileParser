using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebParserCore.Controllers;
using WebParserCore.FilePars;
using WebParserCore.Models;

namespace WebParserCore
{
    public class FileWorker
    {
        private Resposne resp;
        private readonly string[] formats = { "text/plain", "application/msword", "application/pdf"};
        private IFileParser parser;
        private IFormFile File;
        private string way = HomeController.mainPath;
        private int n = -1;

        public FileWorker(IFormFile file, Resposne resp)
        {
            this.resp = resp;
            this.File = file;
        }

        public async Task<bool> WorkFileAsync()
        {
            if(!CheckFileFormat(out n)) return false ;

            await WriteFileToFolderAsync();
            Trace.WriteLine(resp.FileLoc.Name);
            Trace.WriteLine(resp.FileLoc.Path);

            SetParser();
            ImageBuffer.Clear();
            if (!Parse()) return false; 
            return true;
        }

        private bool CheckFileFormat(out int n)
        {
            var format = File.ContentType;

            if (format == "application/octet-stream")
            {
                if (File.FileName.EndsWith(".fb2")) { n = 3; return true; }
                else { n = -1; return false; }
                    
            }
            else
            {
                n = -1;
                foreach (var f in formats)
                {
                    n++;
                    if (format == f) return true;
                }
                HomeController.Message = "Помилка! \n\r Неправильний формат файлу!";
                return false;
            }
        }

        private async Task WriteFileToFolderAsync()
        {
            string path = Path.Combine(way, "files");
            
            string fileName = File.FileName;
            
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await File.CopyToAsync(fileStream);
                }
                resp.FileLoc = new FileHelper(fileName, Path.Combine(path, fileName));
                Resposne.resp = resp;
            }
            catch (System.IO.IOException e)
            {
                System.IO.File.Delete(Path.Combine(path, fileName));
                System.IO.File.Move(fileName.Substring(0, fileName.Length - 4), Path.Combine(path, fileName));
                resp.FileLoc = new FileHelper(fileName, Path.Combine(path, fileName));
                Resposne.resp = resp;
            }
            
        }

        private void SetParser()
        {
            string path = resp.FileLoc.Path;
            string name = resp.FileLoc.Name;
            switch (n)
            {
                case 0:
                    parser = new TXTParser(path, name, resp);
                    break;
                case 1:
                    parser = new DOCParser(path, name, resp);
                    break;
                case 2:
                    parser = new PDFParser(path, name, resp);
                    break;
                case 3:
                    parser = new FB2Parser(path, name, resp);
                    break;
            }
        }

        private bool Parse()
        {
            ImageBuffer.Clear();
            if (parser.Parse())
            {
                resp.File = new FileDesc(ImageBuffer.GetImages(), null);
                return true;
            }
            else return false;
        }
    }
}