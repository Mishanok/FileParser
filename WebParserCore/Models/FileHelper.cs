using System;

namespace WebParserCore.Models
{
    public class FileHelper
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreationTime { get; set; }

        public FileHelper(string name, string path)
        {
            this.Name = name;
            this.Path = path;
            CreationTime = DateTime.Now;
        }
    }
}