using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers
{
    public class InfoController : Controller
    {
        public string about()
        {


            return "MusicInfo";
        }
    }
}