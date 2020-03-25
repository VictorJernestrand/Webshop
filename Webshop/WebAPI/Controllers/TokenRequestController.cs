using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        [Route("renew")]
        [HttpPost]
        [Produces("text/plain")]
        public async Task<IActionResult> RenewToken(APIPayload jwtToken)
        {
            try
            {
                // Validate token...
                var validToken = ValidateToken(jwtToken.Token);
                if (validToken.Identity.IsAuthenticated)
                {
                    var refreshToken = Guid.Parse(validToken.FindFirst("RefreshToken")?.Value);
                    var email = validToken.FindFirst("UserEmail")?.Value;

                    // Get user from database based on email and refresh token
                    var user = await _context.Users.Where(x => x.Email == email && x.RefreshToken == refreshToken).FirstOrDefaultAsync();

                    // User has a role?
                    bool isAdmin = await userManager.IsInRoleAsync(user, "Admin");
                    var role = (isAdmin) ? "Admin" : "";

                    // Create token and store new refreshtoken in database
                    TokenCreatorService tokenService = new TokenCreatorService(_context, _config);
                    var token = tokenService.CreateToken(user, role);

                    return Ok(token);
                }

                throw new SecurityTokenInvalidSignatureException();

            }
            catch (SecurityTokenExpiredException ex)
            {
                return BadRequest(ex);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex);
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                return BadRequest(ex);
            }
            catch (SecurityTokenEncryptionKeyNotFoundException ex)
            {
                return BadRequest(ex);
            }

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

    }
}