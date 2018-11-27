using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebParserCore.Models;

namespace WebParserCore.Controllers
{
    public class HomeController : Controller
    {
        public static Resposne resp;
        TetxWorker tWorker;
        FileWorker fWorker;
        IHostingEnvironment environment;
        public static string mainPath;
        public new static IFormFile File;
        public static string Message;

        public HomeController(IHostingEnvironment appEnvironment)
        {
            
            environment = appEnvironment;
            mainPath = environment.WebRootPath;
        }

        [HttpGet]
        public IActionResult Index()
        {
            Message = "";
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> Index(string title, string user_login, string user_id, string date, string text, IFormFile file)
        {
            resp = new Resposne();

            Initializer.Init(title, user_login, user_id, date, text, file);
            File = file;

            if (Initializer.forf == 0)
            {
                fWorker = new FileWorker(File, resp);
                if (await fWorker.WorkFileAsync() == true)
                {
                    if (Sender.Send(resp, Initializer.forf).Result == true)
                    {
                        Message = "OK!";
                        ViewBag.Message = Message;
                        return Redirect("http://mail.teoset.com:8080/");
                    }
                    else {ViewBag.Message = Message; return View(); }
                }

                else { ViewBag.Message = Message; return View(); }
            }
            else if (Initializer.forf == 1)
            {
                tWorker = new TetxWorker(resp);
                tWorker.WorkText();
                if (Sender.Send(resp, Initializer.forf).Result == true) {Message = "OK!"; ViewBag.Message = Message; return Redirect("http://mail.teoset.com:8080/"); }
                else { ViewBag.Message = Message; return View(); }
            }
            else
            { ViewBag.Message = Message; return View(); }
        }

        [Route("Home/Error/404")]
        public IActionResult Error404()
        {
            return Redirect("mail.teoset.com:8080");
        }
    }
}
