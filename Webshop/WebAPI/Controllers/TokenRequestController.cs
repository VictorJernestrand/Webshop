using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Context;
using WebAPI.Domain;
using WebAPI.Models.Data;
using WebAPI.Services;


namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenRequestController : ControllerBase
    {
        private readonly WebAPIContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<User> userManager;

        public TokenRequestController(WebAPIContext context, IConfiguration config, UserManager<User> userManager)
        {
            this._context = context;
            this._config = config;
            this.userManager = userManager;
        }

        // api/renewtoken
        [AllowAnonymous]
        [Route("new")]
        [HttpPost]
        //[Produces("text/plain")]
        public async Task<IActionResult> RenewToken(APIPayload payload)
        {
            try
            {
                if (string.IsNullOrEmpty(payload.Token))
                    return BadRequest();

                // Validate token...
                var validToken = ValidateToken(payload.Token);
                if (validToken.Identity.IsAuthenticated)
                {
                    //var refreshToken = Guid.Parse(validToken.FindFirst("RefreshToken")?.Value);
                    var email = validToken.FindFirst("UserEmail")?.Value;

                    // Get user from database based on email and refresh token
                    var user = await _context.Users.Where(x => x.Email == email && x.RefreshToken == Guid.Parse(payload.RefreshToken)).FirstOrDefaultAsync();
                    var token = await BakeNewToken(user);

                    return Ok(token);
                }

                throw new SecurityTokenInvalidSignatureException();

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [AllowAnonymous]
        [Route("refresh")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(APIPayload payload)
        {
            // User exist?
            var user = _context.Users.Where(x => x.Email == payload.UserEmail && x.RefreshToken == Guid.Parse(payload.RefreshToken)).FirstOrDefault();

            if (user != null)
            {
                // Create token and store new refreshtoken in database
                var newTokenAndRefreshToken = await BakeNewToken(user);
                return Ok(newTokenAndRefreshToken);
            }
            else
                return Unauthorized();
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _config["JWT:Issuer"],
                ValidAudiences = new[] { _config["JWT:Issuer"] },
                ValidateIssuerSigningKey = true,
                ValidateActor = false,
                ValidateLifetime = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]))
            };

            var handler = new JwtSecurityTokenHandler();
            var result = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

            return result;
        }

        private async Task<APIPayload> BakeNewToken(User user)
        {
            bool isAdmin = await IsUserAdminAsync(user);

            TokenCreatorService tokenService = new TokenCreatorService(_context, _config);
            var newPayload = tokenService.CreateToken(user, isAdmin);

            return newPayload;
        }

        private async Task<bool> IsUserAdminAsync(User user)
            => await userManager.IsInRoleAsync(user, "Admin");
    }
}