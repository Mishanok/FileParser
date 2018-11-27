using Microsoft.AspNetCore.Http;
using System;
using WebParserCore.Controllers;
using WebParserCore.Models;

namespace WebParserCore
{
    public static class Initializer
    {
        private static Resposne resp;
        private static string title;
        private static string user_login;
        private static string user_id;
        private static DateTime Date;
        private static string text;
        private static IFormFile File;
        public static int? forf;

        public static bool Init(string titl, string user_logn, string usr_id, string dat, string txt, IFormFile ifile)
        {
            resp = HomeController.resp;
            title = titl;
            user_login = user_logn;
            user_id = usr_id;
            if(!DateTime.TryParse(dat, out Date)) Date = DateTime.Now;
            text = txt;
            File = ifile;
            if(string.IsNullOrEmpty(text) && ifile == null) { HomeController.Message = "Помилка! \n\r Не введено жодне значення!"; return false; }
            if (!IsTwoFieldFilled(out forf)) {HomeController.Message = "Помилка! \n\r Не можна вводити текст і надсилати файл одночасно!"; return false; }
            else
            {
                InitialResp();
                return true;
            }

        }

        private static void InitialResp()
        {
            resp.Title = title;
            if (string.IsNullOrEmpty(user_login))
            {
                user_login = UniqueID.RandomString(8);
                resp.Additional = "off";
            }
            resp.Login = user_login;
            resp.Additional = "us_on";
            if (user_id != "0")
            {
                int.TryParse(user_id, out int tmp);
                resp.ID = tmp;
            }
            else
            {
                resp.ID = UniqueID.GetID();
            }
            resp.Date = Date;
            TextChecker.Check(text);
            resp.Text = new Text { Content = text, UniqeId = UniqueID.GetID() };
        }

        private static bool IsTwoFieldFilled(out int? forf)
        {
            if (File != null && text != null)
            {
                forf = null;
                return false;
            }
            else if (File != null) { forf = 0; return true; } //file
            else // text
            {
                forf = 1;
                return true;
            }
        }

        
    }
}