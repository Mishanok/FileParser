using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore
{
    public static class JSONCreator
    {
        private static string path ;
        private static string name;
        private static string way = HomeController.mainPath;


        public static void CreateJSON(TextDesc tdesc)
        {
            path = tdesc.Path;
            name = tdesc.Name;
            string text = File.ReadAllText(path);
            MakeJSON(GetArray(text));
        }

        private static void MakeJSON(string[] words)
        {
            string jsonPath = Path.Combine(way, "json_txt", name.Remove(name.Length - 4) + ".json");
            if (!Directory.Exists(Path.Combine(way, "json_txt"))) Directory.CreateDirectory(Path.Combine(way, "json_txt"));
            using (StreamWriter file = File.CreateText(jsonPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, words);
                HomeController.resp.JSONFilePath = jsonPath;
            }
        }  

        private static string[] GetArray(string text)
        {
            return text.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        }
    }
}