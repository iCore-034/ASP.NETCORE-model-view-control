using ASP.NETCORE_model_view_control.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DataNs;

namespace ASP.NETCORE_model_view_control.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration ConfigurationJSON;

        public HomeController(IConfiguration configuration)
        {
            ConfigurationJSON = configuration;
        }
        // ---------------------------------------- СТРАНИЦА INDEX ---------------------------------------- 
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        // ---------------------------------------- СТРАНИЦА PRIVACY ---------------------------------------- 
        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
        // ----------------------------------------  СТРАНИЦА REG ---------------------------------------- 
        [HttpGet]

        public IActionResult Registration(string? login, string? password)
        {
            Data.userLogin = login;
            Data.userPassword = password;
            ToServerDBConnection tsc = new ToServerDBConnection();
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(login))
            {
                Data.userLoginPassError = "Login or password field is empty!";
                return Redirect("~/Home/Privacy/");
            }
            else if (password.Length < 4)
            {
                Data.userLoginPassError = "Password is too simple!";
                return Redirect("~/Home/Privacy");
            }
            else
            {
                if (tsc.Send(false))
                {
                    return Redirect("~/Home/Index/");
                }
                else
                {
                    return Redirect("~/Home/Privacy");
                }
            }
        }
        // ---------------------------------------- СТРАНИЦА AUTH ---------------------------------------- 
        [HttpPost]
        public IActionResult Redirection(string? login, string? password)
        {
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(login))
            {
                Data.userLoginPassError = "Login or password field is empty!";
                return Redirect("~/Home/Index/");

            }
            else if (password.Length < 4)
            {
                Data.userLoginPassError = "Password is too simple!";
                return Redirect("~/Home/Index");
            }
            else
            {
                Data.userLogin = login;
                Data.userPassword = password;
                ToServerDBConnection tsc = new ToServerDBConnection();
                if (tsc.Send(true))
                {
                    return View();

                }
                else
                {
                    Data.userLoginPassError = "Wrong data";
                    return Redirect("~/Home/Index");

                }
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}