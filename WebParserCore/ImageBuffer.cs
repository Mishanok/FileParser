using System.Collections.Generic;
using WebParserCore.Models;

namespace WebParserCore
{
    public static class ImageBuffer
    {
        public static int Count { get
            {
                return Images.Count;
            } }

        private static List<ImageInfo> Images { get; set; } = new List<ImageInfo>();

        public static void AddElement(ImageInfo info)
        {
            Images.Add(info);
        }

        public static List<ImageInfo> GetImages()
        {
            return Images;
        }

        public static void Clear()
        {
            Images.Clear();
        }
    }
}