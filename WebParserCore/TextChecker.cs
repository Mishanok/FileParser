using System.Text;
using System.Text.RegularExpressions;

namespace WebParserCore
{
    public static class TextChecker
    {
        public static string Check(StringBuilder text)
        {
            if (text == null) return null;
            string txt = text.ToString();
            Check(txt);
            return txt;
        }

        public static void Check(string text)
        {
            if (text == null) return;
            CheckScripts(text);
            CheckTags(text);
        }

        private static void CheckScripts(string text)
        {
            var reg = new Regex(@"<script[^>]*>[\s\S]*?</script>");
            text = reg.Replace(text, "");
        }

        private static void CheckTags(string text)
        {
            text = Regex.Replace(text, "<[^>]+>", string.Empty);
        }
    }
}