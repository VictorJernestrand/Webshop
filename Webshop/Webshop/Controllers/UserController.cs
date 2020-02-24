using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Data;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class UserController : Controller
    {
        private readonly WebshopContext context;
        private readonly DatabaseCRUD db;

        public RegisterUserModel RegisterUserModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public UpdateUserPasswordModel UpdateUserPassword { get; set; }
        public EditUserInfoModel EditUserInfoModel { get; set; }



        private UserManager<User> UserMgr { get; }
        private SignInManager<User> SignMgr { get; }
        private RoleManager<AppRole> RoleMgr { get; }


        public UserController(WebshopContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<AppRole> roleManager)
        {
            // Get database context and connection
            this.context = context;

            // Instantiate a new CRUD-object
            db = new DatabaseCRUD(context);

            // Instantiate Auth-services for managing User authorization
            UserMgr = userManager;
            SignMgr = signInManager;
            RoleMgr = roleManager;
        }


        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        
        // Register new customer
        public ActionResult Register()
        {
            return View(RegisterUserModel);
        }
        

        // Login view
        public ActionResult Login()
        {
            return View(LoginModel);
        }


        // View Orders
        [Authorize]
        [HttpGet]
        public ActionResult Orders()
        {
            return View();
        }


        // Login view
        [Authorize]
        public ActionResult UpdateLogin()
        {
            return View(UpdateUserPassword);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> EditUser()
        {
            // Get user information
            User user = new User();

            user = await UserMgr.GetUserAsync(HttpContext.User); //context.Users.Where(x => x.UserName == HttpContext.User.Identity.Name).FirstOrDefault();

            EditUserInfoModel = new EditUserInfoModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                StreetAddress = user.StreetAddress,
                ZipCode = user.ZipCode,
                City = user.City
            };

            return View(EditUserInfoModel);
        }


        // POST: User/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind]LoginModel model)
        {
            try
            {
                // First, validate the form. Did the user provide expected data such as email and password?
                if (ModelState.IsValid)
                {
                    // Make a sign-in request!
                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await SignMgr.PasswordSignInAsync(model.UserEmail, model.UserPassword, model.RememberUser, false);

                    // Did the user submit correct email and password?
                    if (signInResult.Succeeded)
                    {
                        // Get user information from database
                        User user = await db.GetUserByUserEmail(model.UserEmail);

                        // Is user admin?
                        bool isAdmin = await UserMgr.IsInRoleAsync(user, "Admin");
                        if (isAdmin)
                        {
                            // Login succesfull! Redirect to main page :)
                            return RedirectToAction("Index", "Admin");
                        }

                        // Bake a new session-cookie with the User's name as the main ingredient ;)
                        HttpContext.Session.SetString(SessionCookies.USER_NAME, user.FirstName + " " + user.LastName);

                        // Login succesfull! Redirect to main page :)
                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.LoginResult = "Felaktiga inloggningsuppgifter! Försök igen!";
                }

                return View(model);
            }
            catch
            {
                return View(model);
            }
        }


        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind]RegisterUserModel model)
        {
            try
            {
                // Was the registration form filled out correctly?
                if (ModelState.IsValid)
                {
                    // Initialize a new User-object and populate it with the provided user data
                    User newUser = new User()
                    {
                        UserName = model.Email, // Must be filled due to the autogenerated fields in the AspNetUsers table in the database
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Password = model.Password
                    };

                    // Store the new user in the database
                    IdentityResult result = await UserMgr.CreateAsync(newUser, newUser.Password);

                    // Redirect the user to the login page
                    if (result.Succeeded)
                    {
                        TempData["RegisterSuccess"] = "Ditt konto har skapats!";
                        return RedirectToAction(nameof(Login));
                    }

                    // Loop through the errors and customize errormessages
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateEmail")
                            ModelState.AddModelError("Email", "Epostadressen används redan");

                        if (error.Code == "PasswordTooShort" ||
                            error.Code == "PasswordRequiresNonAlphanumeric" ||
                            error.Code == "PasswordRequiresLower" ||
                            error.Code == "PasswordRequiresUpper")
                            {
                                ModelState.AddModelError("Password", "Lösenordet måste bestå av minst 6 tecken och innehålla en stor och liten bokstav, en siffra + specialtecken.");
                            }

                        if (error.Code == "PasswordMismatch")
                            ModelState.AddModelError("Password", "Lösenorden stämmer inte överrens");
                    }

                }

                // The registration form contains errors, display the view again along with the error information
                return View(model);

            }
            catch
            {
                return View();
            }
        }

        /*
        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                int rowsEffected = await db.UpdateAsync(User);
                return RedirectToAction(nameof(Edit));
            }
            catch
            {
                return View();
            }
        }
        */

        // Login view
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateLogin([Bind]UpdateUserPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // Get current user from database
                User user = await UserMgr.GetUserAsync(HttpContext.User);

                // Replace current password with new password
                IdentityResult result = await UserMgr.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                // Was passwordreplacement successful?
                if (result.Succeeded)
                {
                    TempData["PasswordSuccess"] = "Lösenordet har uppdaterats!";
                    return RedirectToAction(nameof(UpdateLogin));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "PasswordMismatch")
                        {
                            ModelState.AddModelError("CurrentPassword", "Felaktigt lösenord");
                        }

                        if (error.Code == "PasswordTooShort" ||
                        error.Code == "PasswordRequiresNonAlphanumeric" ||
                        error.Code == "PasswordRequiresLower" ||
                        error.Code == "PasswordRequiresUpper")
                        {
                            ModelState.AddModelError("NewPassword", "Lösenordet måste bestå av minst 6 tecken och innehålla en stor och liten bokstav, en siffra + specialtecken.");
                        }
                    }
                }
            }

            // Return model
            return View(model);
        }


        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> EditUser([Bind]EditUserInfoModel model)
        {
            if (ModelState.IsValid)
            {
                // Fetch user from database
                User user = await UserMgr.GetUserAsync(HttpContext.User);

                // Update user with the new information
                user.UserName       = model.Email;
                user.Email          = model.Email;
                user.FirstName      = model.FirstName;
                user.LastName       = model.LastName;
                user.PhoneNumber    = model.PhoneNumber;
                user.StreetAddress  = model.StreetAddress;
                user.ZipCode        = model.ZipCode;
                user.City           = model.City;

                // Save changes
                var result = await UserMgr.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["UpdateSuccess"] = "Din information har uppdaterats!";
                    return RedirectToAction(nameof(EditUser));
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName" || error.Code == "DuplicateEmail")
                        {
                            ModelState.AddModelError("Email", "Epostadressen används redan");
                            break;
                        }
                    }
                }
            }

            return View(model);
        }


        public IActionResult LogOut()
        {
            SignMgr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}