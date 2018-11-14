using System;

namespace WebParserCore.Models
{
    public class Resposne
    {
        public string Login { get; set; } 
        public int ID { get; set; }
        public int TextID { get; set; }
        public string Title { get; set; }
        public TextDesc TDesc { get; set; }
        public FileDesc File { get; set; }
        public FileHelper FileLoc { get; set; }
        public DateTime Date { get; set; } 
        public Text Text { get; set; }
        public string Additional { get; set; }
        public string JSONFilePath { get; set; }

        public static Resposne resp;
    }
}