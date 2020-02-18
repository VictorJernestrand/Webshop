using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class AdminController : Controller
    {

        private UserManager<User> UserMgr { get; set; }
        private RoleManager<AppRole> RoleMgr { get; set; }

        public AdminController(UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            UserMgr = userManager;
            RoleMgr = roleManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CreateRole([Bind]UserRoleModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var result2 = await RoleMgr.CreateAsync(new AppRole("Admin"));
                    //var user = await UserMgr.
                    var result = UserMgr.Users.Where(x => x.Id == 1).FirstOrDefault();
                    var test = await UserMgr.AddToRoleAsync(result, "Admin"); 
                }
                catch
                {
                    //...
                }
            }
        }*/

    }
}