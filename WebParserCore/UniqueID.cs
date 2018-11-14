using System;
using System.Linq;

namespace WebParserCore
{
    public static class UniqueID
    {
        private static Random random = new Random();

        public static int GetID(int max)
        {
            return random.Next(max);
        }

        public static int GetID()
        {
            return random.Next();
        }
        
        public static char GetID(char f)
        {
            return (char)(random.Next(0, 26));
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}