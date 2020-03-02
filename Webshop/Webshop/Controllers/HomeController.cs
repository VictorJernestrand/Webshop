using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly DatabaseCRUD db;
        private readonly WebshopContext context;

        public new User User { get; set; }

        private UserManager<User> UserMgr { get; }

        public HomeController(WebshopContext context, UserManager<User> userManager, ILogger<HomeController> logger)
        {
            this.context = context;
            db = new DatabaseCRUD(context);
            UserMgr = userManager;
            _logger = logger;
        }

        private void GetLoggedInUserAsync()
        {
            
            //User = db.GetByIdAsync<User>(UserMgr.GetUserId(HttpContext.User));
        }

        /*public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        public IActionResult Index()
        {
            //User = await UserMgr.GetUserAsync(HttpContext.User);
            //ViewData["UserName"] = User.FirstName;// HttpContext.Session.GetString(SessionCookies.USER_NAME);
            //var result = context.Categories.ToList();
            return View(User);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Slide()
        {
            return View();
        }
       
    }
}
