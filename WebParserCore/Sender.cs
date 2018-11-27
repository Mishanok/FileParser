using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebParserCore.Models;
using System.Net.Http;
using System.Net;

namespace WebParserCore
{
    public static class Sender
    {
        private static List<byte[]> images = new List<byte[]>();
        private static string[] names;
        private static Dictionary<string, string> formFields;

        public static async Task<bool> Send(Resposne resp, int? f)
        {
            string url = "https://teoset.com/blocks/generators/reader_sys.php";

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" +
                                    boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            Stream memStream = new System.IO.MemoryStream();

            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                    boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" +
                                                                        boundary + "--");


            string formdataTemplate = "\r\n--" + boundary +
                                        "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            InitializeFormFields(resp);

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }

            string headerTemplate =
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                "Content-Type: application/octet-stream\r\n\r\n";

            var imageArr = images.ToArray();

            //send txt file
            if (f == 1)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header0 = string.Format(headerTemplate, "txt", resp.TDesc.Path);
                var headerbytes0 = System.Text.Encoding.UTF8.GetBytes(header0);

                memStream.Write(headerbytes0, 0, headerbytes0.Length);

                using (var fileStream = new FileStream(resp.TDesc.Path, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
            else if(f == 0)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header0 = string.Format(headerTemplate, resp.FileLoc.Name, resp.FileLoc.Path);
                var headerbytes0 = System.Text.Encoding.UTF8.GetBytes(header0);

                memStream.Write(headerbytes0, 0, headerbytes0.Length);

                using (var fileStream = new FileStream(resp.FileLoc.Path, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[16000];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            //send JSON file
            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            var header1 = string.Format(headerTemplate, "jsonFile", resp.JSONFilePath);
            var headerbytes1 = System.Text.Encoding.UTF8.GetBytes(header1);

            memStream.Write(headerbytes1, 0, headerbytes1.Length);

            using (var fileStream = new FileStream(resp.JSONFilePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[1024];
                var bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
            }

            //send images
            for (int i = 0; i < images.Count; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "uplTheFile", ImageBuffer.GetImages().ToArray()[i].Path);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                var image = ImageBuffer.GetImages().ToArray()[i].Path;

                using (var fileStream = new FileStream(image, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (var response = request.GetResponse())
            {
                Stream stream2 = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                Trace.WriteLine(reader2.ReadToEnd());
                return true;
            }
        }

        private static void InitializeFormFields(Resposne resp)
        {
//1 логин, 
//2 id пользователя,
//3 уникальный id для текста который не будет повторяться при последующих обработках, не важно какой длинны. Он должен сгенерироваться в обработчике.
//4 название из поля title,
//5(если вставлен текст) текстовый файл в формате .txt; если вставлен файл то оригинал файла.
//6 json файл
//7 количество картинок(просто число)
//8 Все картинки
//9 обозначение к каждой картинке
//10 дата
//11 Если логин равен 0 то 'off' если пользователь существует то 'us_on'
            formFields = new Dictionary<string, string>();
            formFields.Add("user_login", resp.Login);
            formFields.Add("user_id", resp.ID.ToString());
            formFields.Add("txt_id", resp.Text.UniqeId.ToString());
            formFields.Add("title", resp.Title);
            formFields.Add("img_count", ImageBuffer.Count.ToString());
            formFields.Add("date", DateTime.Now.ToShortDateString());
            formFields.Add("add", "us_on");
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