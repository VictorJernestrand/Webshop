using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAPI.Domain;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class UserController : Controller
    {
        public RegisterUserModel RegisterUserModel { get; set; }
        public LoginModel LoginModel { get; set; }
        public UpdateUserPasswordModel UpdateUserPassword { get; set; }
        public EditUserInfoModel EditUserInfoModel { get; set; }

        private WebAPIHandler webAPI;
        private readonly IConfiguration config;
        private WebAPIToken webAPIToken;

        public UserController(WebAPIToken webAPIToken, WebAPIHandler webAPIHandler, IConfiguration config)
        {
            // Instantiate a new WebAPIHandler object
            this.webAPI = webAPIHandler;
            this.config = config;
            this.webAPIToken = webAPIToken;
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
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("index", "Home");

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
            var email = User.Identity.Name;

            var token = await webAPIToken.New();
            var user = await webAPI.GetOneAsync<User>(ApiURL.USERS + email, token);
            EditUserInfoModel = new EditUserInfoModel()
            {
                Email           = user.Email,
                FirstName       = user.FirstName,
                LastName        = user.LastName,
                PhoneNumber     = user.PhoneNumber,
                StreetAddress   = user.StreetAddress,
                ZipCode         = user.ZipCode,
                City            = user.City
            };

            return View(EditUserInfoModel);
        }

        // POST: User/Create
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind]LoginModel model)
        {

            if (ModelState.IsValid)
            {
                var apiResult = await webAPI.PostAsync(model, ApiURL.USERS_LOGIN);

                if (apiResult.Status.IsSuccessStatusCode)
                {
                    await SetAuthCookie(apiResult.APIPayload, model.RememberUser);
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.LoginResult = "Felaktiga inloggningsuppgifter! Försök igen!";
            return View(model);

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
                    var apiResult = await webAPI.PostAsync<RegisterUserModel>(model, ApiURL.USERS_REGISTER);

                    if (!apiResult.Status.IsSuccessStatusCode && apiResult.ResponseContent.Length > 0)
                    {
                        var errors = JsonSerializer.Deserialize<RegisterErrorCodes>(apiResult.ResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                        if (errors.Email[0].Length > 0)
                            ModelState.AddModelError("Email", "Adressen används redan");
                    }
                    else
                    {
                        TempData["RegisterSuccess"] = "Ditt konto har skapats!";
                        return RedirectToAction(nameof(Login));
                    }
                }

                return View(model);

            }
            catch
            {
                return View();
            }
        }

        // Login view
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateLogin([Bind]UpdateUserPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var apiResult = await webAPI.UpdateAsync<UpdateUserPasswordModel>(model, ApiURL.USERS_LOGIN_UPDATE + User.Identity.Name);

                if (apiResult.Status.IsSuccessStatusCode)
                {
                    await SetAuthCookie(apiResult.APIPayload);
                    TempData["PasswordSuccess"] = "Lösenordet har uppdaterats!";
                    return RedirectToAction(nameof(UpdateLogin));
                }
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> EditUser([Bind]EditUserInfoModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    StreetAddress = model.StreetAddress,
                    ZipCode = model.ZipCode,
                    City = model.City
                };

                var apiResult = await webAPI.UpdateAsync<User>(user, ApiURL.USERS_INFO_UPDATE + User.Identity.Name);

                if (apiResult.Status.IsSuccessStatusCode)
                {
                    // If email was updated, update token cookie and authcookie with new criterias!
                    if (!User.Identity.Name.Equals(model.Email))
                    {
                        await SetAuthCookie(apiResult.APIPayload);
                    }

                    TempData["UpdateSuccess"] = "Din information har uppdaterats!";
                    return RedirectToAction(nameof(EditUser));
                }
                else
                {
                    var errors = JsonSerializer.Deserialize<List<ErrorCodes>>(apiResult.ResponseContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                    if (errors.Any(x => x.Code == "DuplicateEmail"))
                        ModelState.AddModelError("Email", "Adressen används redan");
                }
            }

            return View(model);
        }


        public async Task<IActionResult> LogOut()
        {
            // Remove shoppingcart session cookie!
            HttpContext.Session.Remove(config["CartSessionCookie:Name"]);

            // Remove shoppingcart session cookie!
            HttpContext.Session.Remove(config["JwtSessionToken:Name"]);

            // Remove TokenCookie
            Response.Cookies.Delete(config["RefreshToken:Name"]);

            // SignMgr.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        // TODO: Add User object to PayLoad class

        public async Task SetAuthCookie(APIPayload payload, bool persistent = true)
        {
            // Bake Token Refresh cookie
            webAPIToken.TokenRefreshCookie = payload.RefreshToken;

            // Bake Token-Cookie
            webAPIToken.SessionTokenRefresh = payload.Token;

            // Extract payload from token cookie
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(payload.Token);

            var userEmail = token.Claims.Where(x => x.Type == "UserEmail")
                .Select(x => x.Value)
                .FirstOrDefault()
                .ToString();

            var userName = token.Claims.Where(x => x.Type == "UserName")
                .Select(x => x.Value)
                .FirstOrDefault()
                .ToString();

            var userRole = token.Claims.Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .FirstOrDefault()
                .ToString();

            // Set up Claims for ASP authentication cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Name, userEmail),
                new Claim("UserName", userName),
                new Claim(ClaimTypes.Role, userRole)
            };

            // Do user want to be remembered?
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = persistent,
                ExpiresUtc = DateTime.UtcNow.AddMonths(int.Parse(config["AuthCookie:Expire"]))
            };

            // Bake authentication cookie!
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
            
        }

    }

    public class RegisterErrorCodes
    {
        public List<string> Email { get; set; }
    }

    public class ErrorCodes
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

}