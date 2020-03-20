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
    public class UserTestController : ControllerBase
    {
        private readonly WebAPIContext _context;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<AppRole> roleManager;
        private readonly IConfiguration _configure;

        public UserTestController(WebAPIContext context, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<AppRole> roleManager, IConfiguration configure)
        //public UserTestController(WebAPIContext context, IConfiguration configure)
        {
            _context = context;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this._configure = configure;
        }


        // GET: api/UserTest
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }


        // GET: api/UserTest/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/UserTest/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserTest
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<User>> PostUser(User user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}


        // POST: api/UserTest
        [HttpPost]
        [Produces("text/plain")]
        public async Task<ActionResult<User>> LoginUser(LoginModel model)
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

                string admin = (isAdmin) ? "Admin" : null;

                // Construct JWT token
                TokenCreatorService tokenService = new TokenCreatorService(_context, _configure);
                var result = tokenService.CreateToken(user, admin);
                //AuthenticationCredentials token = new AuthenticationCredentials()
                //{
                //    Token = result,
                //};

                return Ok(result);
            }

            else
                return Unauthorized();
            return Ok();
        }



        // DELETE: api/UserTest/5
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
