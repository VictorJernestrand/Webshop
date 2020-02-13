using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Data;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class UserController : Controller
    {
        //WebshopContext webshopContext = new WebshopContext();
        DatabaseCRUD db = new DatabaseCRUD();

        [BindProperty]
        public new User User { get; set; } = new User();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        // Register new customer
        public ActionResult Register()
        {
            int? userId = HttpContext.Session.GetInt32(SessionCookies.USER_ID);

            return View(User);
        }

        // Login view
        public ActionResult Login()
        {
            return View(User);
        }

        [Authorize]
        public async Task<ActionResult> Edit()
        {
            int userId = 2; // For testing purposes
            User = await db.GetAsync<User>(userId);

            return View(User);
        }


        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult LoginUser(IFormCollection collection)
        public ActionResult Login(User model)
        {
            try
            {
                if (ModelState.IsValid)
                {/*
                    // Generate a hashed string out of the users password using the
                    // extension method GenerateHash() in the PasswordHandler class
                    //string hashedUserPass = User.Password.GenerateHash();

                    // Check the users cridentials for a match in the database
                    var userExist = db.GetUserByUserCredentials(User.Email, hashedUserPass);

                    // If user exist, set the initialize the session cookies so we know
                    // that the user is logged in!
                    if (userExist != null)
                    {
                        // Bake a session cookie with the users Id
                        HttpContext.Session.SetInt32(SessionCookies.USER_ID, userExist.Id);

                        // Redirect the user to some page here...
                    }
                    */
                    return RedirectToAction(nameof(Register));
                }
                else
                {
                    //return BadRequest(ModelState);
                    return View(model);
                    //return RedirectToAction(nameof(Login));
                }
            }
            catch
            {
                return View();
            }
        }


        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(IFormCollection collection)
        {
            try
            {
                // Make a comparisson between password1 and password2 to make sure the passwords match.
                if (collection["Password"][0].ToString() == collection["Password"][1].ToString())
                {
                    // Generate a hashed string out of the password and store the new customer in database
                    //User.Password = User.Password.GenerateHash();
                    int rowsEffected = await db.InsertAsync(User);

                    // Let the user know the registration was successful and ask the user to log in using email and password
                    
                    // Do some redirect here...
                }
                return RedirectToAction(nameof(Register));
            }
            catch
            {
                return View();
            }
        }


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







        /*
        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }*/
    }
}