using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class UserOrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}