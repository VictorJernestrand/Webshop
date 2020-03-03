using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]
        public IActionResult PostMessage([Bind]ContactModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ContactModel newMessage = new ContactModel()
                    {
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Message = model.Message
                    };
                    TempData["MessageSuccess"] = "Tack för ditt meddelande!";
                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }
    }
}