using System.Collections.Generic;

namespace WebParserCore.Models
{
    public class FileDesc
    {
        public int ImagesCount { get
            {
                return Images.Length;
            }
        }
        public ImageInfo[] Images { get; set; }
        public string[] Marks { get; set; }

        public FileDesc(List<ImageInfo> imgs, string[] marks)
        {
            Images = imgs.ToArray();
            Marks = marks;
        }
    }
}