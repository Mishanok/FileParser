using WebParserCore.Models;

namespace WebParserCore.FilePars
{
    public abstract class FileParser : IFileParser
    {
        internal string path;
        internal string name;
        internal Resposne resp;
        internal TetxWorker tworker;

        public FileParser(string path, string name, Resposne resp)
        {
            this.path = path;
            this.name = name;
            this.resp = resp;
            tworker = new TetxWorker(resp);
        }

        public abstract bool Parse();
    }
}