using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Context
{
    // User-roles
    enum role { Admin, Manager, User }

    public class AdminAccountAndRoles
    {
        public static async Task Initialize(WebAPIContext context, UserManager<User> userManager, RoleManager<AppRole> roleManager)
        {
            context.Database.EnsureCreated();

            string adminEmail = "admin@webshop.se";
            string password = "Test123!";
            string firstName = "RockStart";
            string lastName = "Administrator";

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


            var admin = await userManager.FindByEmailAsync(adminEmail);

            // Create Admin!
            if (admin == null)
            {
                // Create User with admin-email and password
                User adminUser = new User()
                {
                    FirstName = firstName,
                    LastName = lastName,
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
            else if(admin.FirstName == null || admin.LastName == null)
            {
                admin = await context.Users.Where(x => x.Email == adminEmail).FirstOrDefaultAsync();

                admin.FirstName = firstName;
                admin.LastName = lastName;

                // Store user in database
                var result = await userManager.UpdateAsync(admin);
            }

        }
    }
}
