using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class ErrorController : Controller
    {

        [Route("Error/{statusCode}")]
        public IActionResult CustomErrorCode(int statusCode)
        {
            // Interface IStatusCodeReExecuteFeature gives path and querystring the user tried to access
            var errorInfo = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            // Handle error codes
            if (statusCode == 404)
            {
                ViewBag.ErrorMessage = @$"Skitsidan ""{errorInfo.OriginalPath}"" du försöker komma åt finns inte.";
            }
            else
            {
                ViewBag.ErrorMessage = "Oops, något gick åt helvete. Kontakta support om felet kvarstår.";
            }

            ViewBag.StatusCode = statusCode;
            return View("PageNotFound");
        }

        [Route("Error")]
        public IActionResult UnhandledExceptionHandler()
        {
            // Interface IExceptionHandlerPathFeature gives information about path and exception errors 
            var errorPath = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            ViewBag.ErrorMessage = "Det där sket sig! Det verkar som att du försöker komma åt något som inte finns.";
            return View("PageNotFound");
        }
    }
}