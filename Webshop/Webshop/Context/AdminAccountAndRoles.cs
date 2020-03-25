using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webshop.Models;

namespace Webshop.Context
{
    // User-roles
    enum role { Admin, Manager, User }

    public class AdminAccountAndRoles
    {
        public static async Task Initialize(WebshopContext context, UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminEmail = "admin@webshop.se";
            string password = "Test123!";

            // Admin

            string adminRole = Enum.GetName(typeof(role), role.Admin);
            if (await roleManager.FindByNameAsync(adminRole) == null)
                await roleManager.CreateAsync(new AppRole(adminRole));

            // Manager
            string managerRole = Enum.GetName(typeof(role), role.Manager);
            if (await roleManager.FindByNameAsync(managerRole) == null)
                await roleManager.CreateAsync(new AppRole(managerRole));

            // User
            string userRole = Enum.GetName(typeof(role), role.User);
            if (await roleManager.FindByNameAsync(userRole) == null)
                await roleManager.CreateAsync(new AppRole(userRole));

            // Create Admin!
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                // Create User with admin-email and password
                User adminUser = new User()
                {
                    FirstName = "RockStart",
                    LastName = "Administrator",
                    UserName = adminEmail,
                    Email = adminEmail,
                    Password = password,
                };

                // Store user in database
                var result = await userManager.CreateAsync(adminUser, adminUser.Password);

                // If storage was successful, add a role to the user.
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, adminRole);
                }
            }

        }
    }
}
