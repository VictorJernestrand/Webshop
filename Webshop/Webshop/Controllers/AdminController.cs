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
        public UserRoleModel UserRoleModel { get; set; }

        private UserManager<User> UserMgr { get; set; }
        private RoleManager<IdentityRole<int>> RoleMgr { get; set; }

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            UserMgr = userManager;
            RoleMgr = roleManager;
        }

        public IActionResult Index()
        {
            return View(UserRoleModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task CreateRole([Bind]UserRoleModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //var result = await RoleMgr.CreateAsync(new IdentityRole<int>("Arne"));
                    //var user = await UserMgr.
                    var result = UserMgr.Users.Where(x => x.Id == 1).FirstOrDefault();
                    var test = await UserMgr.AddToRoleAsync(result, "Admin"); 
                }
                catch
                {
                    //...
                }
            }
        }


        [Authorize(Roles = "Admin")]
        public IActionResult TestPage()
        {
            return View();
        }
        /*
        public async Task<IActionResult> Update(string id)
        {
            IdentityRole<int> role = await RoleMgr.FindByIdAsync(id);
            List<User> members = new List<User>();
            List<User> nonMembers = new List<User>();
            foreach (User user in UserMgr.Users)
            {
                var list = await UserMgr.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit)
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }*/

    }
}