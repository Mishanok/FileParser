using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebParserCore.Models;

namespace WebParserCore
{
    public static class Sender
    {
        private static List<byte[]> images = new List<byte[]>();
        private static string[] names;

        public static async Task<bool> Send(Resposne resp)
        {
            var txtFile = File.ReadAllBytes(resp.TDesc.Path);
            var jsonFile = File.ReadAllBytes(resp.JSONFilePath);
            images.Clear();

            int index = 0;

            InitialImages();

            using (var client = new HttpClient())
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(resp.Login), "user_login");
                formData.Add(new StringContent(resp.ID.ToString()), "user_id");
                formData.Add(new StringContent(resp.Text.UniqeId.ToString()), "txt_id");
                if (resp.Title != null) formData.Add(new StringContent(resp.Title), "title");
                else formData.Add(new StringContent("NON"), "title");
                formData.Add(new ByteArrayContent(txtFile, 0, txtFile.Length), "txt_file", resp.TDesc.Name);
                formData.Add(new ByteArrayContent(jsonFile, 0, jsonFile.Length), "json_file", resp.TDesc.Name.Remove(resp.TDesc.Name.Length - 4) + ".json");
                formData.Add(new StringContent(ImageBuffer.Count.ToString()), "img_count");
                foreach (var i in images.ToArray())
                {
                    formData.Add(new ByteArrayContent(i, 0, i.Length), "image" + i, names[index]);
                }
                formData.Add(new StringContent(DateTime.Now.ToShortDateString()), "date");
                formData.Add(new StringContent(resp.Additional), "is_login");
                var response = await client.PostAsync("https://teoset.com/blocks/generators/reader_sys.php/", formData);
                await PostFilesAsync(client, resp);
                await Task.Run(() => ClearVPS(resp));
                if (!response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("Status code - nonSuccess");
                    return false;
                }
                var stream = await response.Content.ReadAsStreamAsync();
                return true;
            }
        }

        private static void ClearVPS(Resposne resp)
        {
            if (resp.FileLoc != null)
                File.Delete(resp.FileLoc.Path);

            File.Delete(resp.JSONFilePath);
            foreach(var i in ImageBuffer.GetImages())
            {
                File.Delete(i.Path);
            }
            File.Delete(resp.TDesc.Path);
        }

        private static void InitialImages()
        {
            int id = 0;
            names = new string[ImageBuffer.Count];

            foreach (var i in ImageBuffer.GetImages())
            {
                images.Add(File.ReadAllBytes(i.Path));
                names[id] = i.Name;
                id++;
            }
        }

        private static async Task<bool> PostFilesAsync(HttpClient client, Resposne resp)
        {
            int index = 0;
            var txtFile = File.ReadAllBytes(resp.TDesc.Path);
            var jsonFile = File.ReadAllBytes(resp.JSONFilePath);

            using (var formData = new MultipartFormDataContent())
            {
                if (resp.FileLoc != null)
                {
                    var mainFile = File.ReadAllBytes(resp.FileLoc.Path);
                    formData.Add(new ByteArrayContent(mainFile, 0 ,mainFile.Length));
                }

                formData.Add(new ByteArrayContent(txtFile, 0, txtFile.Length), "txt_file", resp.TDesc.Name);
                formData.Add(new ByteArrayContent(jsonFile, 0, jsonFile.Length), "json_file", resp.TDesc.Name.Remove(resp.TDesc.Name.Length - 4) + ".json");

                foreach (var i in images.ToArray())
                {
                    formData.Add(new ByteArrayContent(i, 0, i.Length), "image" + i, names[index]);
                }

                var response = await client.PostAsync("https://teoset.com/", formData);
                if (!response.IsSuccessStatusCode)
                {
                    Trace.WriteLine("Status code - nonSuccess");
                    return false;
                }
                var stream = await response.Content.ReadAsStreamAsync();
                return true;
            }
        }

        public static bool SendTextFile(Resposne resp)
        {
            try
            {
                string fileName = "sended_text.txt";
                string path = "D:\\temp\\" + fileName;

                StringBuilder text = new StringBuilder();

                text.AppendLine("User`s Login: " + resp.Login);
                text.AppendLine("ID of text: " + resp.Text.UniqeId);
                text.AppendLine("User`s ID: " + resp.ID);
                text.AppendLine("Title: " + resp.Title);
                text.AppendLine("Txt file path: " + resp.TDesc.Path);
                text.AppendLine("JSON file path: " + resp.JSONFilePath);
                text.AppendLine("User`s Login: " + resp.Login);
                text.AppendLine("Images count: " + ImageBuffer.Count);
                foreach (var i in ImageBuffer.GetImages())
                {
                    int index = 1;
                    text.AppendLine("Image " + index++ + " : " + i.Name);
                }
                text.AppendLine("Date: " + DateTime.Now);
                text.AppendLine("Additional stirng: " + resp.Additional);

                File.WriteAllText(path, text.ToString());
                return true;
            }
            catch (Exception e) {
                Trace.WriteLine(e.Message);
                return false;
            }

        }
    }
}