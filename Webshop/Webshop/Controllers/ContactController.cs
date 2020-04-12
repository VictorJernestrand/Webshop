using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ContactController : Controller
    {
        private readonly IConfiguration _config;
        public ContactModel ContactModel = new ContactModel();
        public ContactController(IConfiguration config)
        {
            this._config = config;
        }
        public IActionResult Index()
        {
            return View(ContactModel);
        }

        //[HttpPost, ActionName("Index")]
        //[ValidateAntiForgeryToken]
      //  public IActionResult PostMessage([Bind]ContactModel model)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            ContactModel newMessage = new ContactModel()
        //            {
        //                Email = model.Email,
        //                FirstName = model.FirstName,
        //                LastName = model.LastName,
        //                PhoneNumber = model.PhoneNumber,
        //                Message = model.Message
        //            };
        //            TempData["MessageSuccess"] = "Tack för ditt meddelande!";
        //            return RedirectToAction(nameof(Index));
        //        }
        //        return View(model);
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}



        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                var body = "<p>Email From: {0} ({1})</p><p>Message:</p><p>{2}</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(_config["Mail:Address"]));
                message.From = new MailAddress(model.Email);
                message.Subject = "Kund klagomål";
                message.Body = string.Format(body, model.FirstName + "" + model.LastName, model.Email, model.Message);
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = _config["Mail:Address"],
                        Password = _config["Mail:Password"]
                    };
                    smtp.Credentials = credential;
                    smtp.Host = _config["Mail:SMTP"];
                    smtp.Port = Convert.ToInt32(_config["Mail:Port"]);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    //TempData["MessageSuccess"] = "Our customer support will into the issue. Thanks for contacting us";
                    //return RedirectToAction("Index");
                }

            }
            else
            {
                return RedirectToAction("Contact", model);
            }
            TempData["MessageSuccess"] = "Our customer support will into the issue. Thanks for contacting us";
            return RedirectToAction("Index");
        }
    
    }
}