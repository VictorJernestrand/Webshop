using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Context;
using WebAPI.Domain;
using WebAPI.Models;
using WebAPI.Models.Data;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly WebAPIContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration _configure;

        public UsersController(WebAPIContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<AppRole> roleManager, IConfiguration configure)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this._configure = configure;
        }


        // GET: api/User
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // GET: api/User/5
        //       [Authorize(Roles = "Admin")]
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            var user = await _context.Users.Where(x => x.Email == email)
                .Select(x => new
                {
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.PhoneNumber,
                    x.StreetAddress,
                    x.ZipCode,
                    x.City
                }).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut]
        [Route("infoupdate/{email}")]
        public async Task<ActionResult<IEnumerable<IdentityError>>> UpdateUserInfo(User user, string email)
        {
            var updateUser = await userManager.FindByNameAsync(email);

            if (updateUser == null)
            {
                return NotFound();
            }

            // Update user with the new information
            updateUser.UserName = user.Email;
            updateUser.Email = user.Email;
            updateUser.FirstName = user.FirstName;
            updateUser.LastName = user.LastName;
            updateUser.PhoneNumber = user.PhoneNumber;
            updateUser.StreetAddress = user.StreetAddress;
            updateUser.ZipCode = user.ZipCode;
            updateUser.City = user.City;

            var result = await userManager.UpdateAsync(updateUser);

            if (result.Succeeded)
            {
                TokenCreatorService tokenService = new TokenCreatorService(_context, _configure);

                var isAdmin = false;
                var newToken = tokenService.CreateToken(updateUser, isAdmin);
                return Ok(newToken);
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPut]
        [Route("loginupdate/{email}")]
        public async Task<IActionResult> UpdateLoginInfo(UpdateUserPasswordModel model, string email)
        {
            if (ModelState.IsValid)
            {
                User user = await userManager.FindByEmailAsync(email);

                // Replace current password with new password
                IdentityResult result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    User updatedUser = await userManager.FindByEmailAsync(email);

                    TokenCreatorService tokenService = new TokenCreatorService(_context, _configure);

                    var isAdmin = false;

                    var payload = tokenService.CreateToken(updatedUser, isAdmin, true);

                    return Ok(payload);
                }
            }

            return Unauthorized(new APIPayload());
        }

        // POST: api/Users
        [HttpPost]
        [Route("Login")]
        //[Produces("text/plain")]
        public async Task<ActionResult<APIPayload>> LoginUser(LoginModel model)
        {
            // Get user by e-mail
            User user = await _context.Users.Where(x => x.Email == model.UserEmail).FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            // Use identity framework to compare passwords in database
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, model.UserPassword, false);

            // If authentication was successful...
            if (signInResult.Succeeded)
            {
                // Is user Admin?
                bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");

                // Construct JWT token
                TokenCreatorService tokenService = new TokenCreatorService(_context, _configure);
                var newToken = tokenService.CreateToken(user, isAdmin, true);

                return Ok(newToken);
            }

            else
                return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<IEnumerable<IdentityError>>> RegisterUser(RegisterUserModel model)
        {
            if (ModelState.IsValid)
            {
                User newUser = new User()
                {
                    UserName = model.Email, // Must be filled due to the autogenerated fields in the AspNetUsers table in database
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password
                };

                // Create user
                IdentityResult result = await userManager.CreateAsync(newUser, newUser.Password);
                IdentityResult role = await userManager.AddToRoleAsync(newUser, "User");

                // If user-creation was successful, give user a 'User' role .
                if (result.Succeeded && role.Succeeded)
                {
                    return Ok();
                }

                // Generate error messages and return ModelState
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
                }

                return BadRequest(ModelState);
            }

            return Unauthorized();
        }




        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
