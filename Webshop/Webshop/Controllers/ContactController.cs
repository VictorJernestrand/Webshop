using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ContactController : Controller
    {
        public ContactModel ContactModel { get; set; }
        public IActionResult Index()
        {
            return View(ContactModel);
        }
    }
}