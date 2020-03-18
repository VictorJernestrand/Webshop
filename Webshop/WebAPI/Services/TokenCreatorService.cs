using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Context;
using WebAPI.Domain;
using WebAPI.Models;
using WebAPI.Models.Data;

namespace WebAPI.Services
{
    public class TokenCreatorService
    {
        private readonly WebAPIContext _context;
        private readonly IConfiguration _configure;

        public TokenCreatorService(WebAPIContext context, IConfiguration configure)
        {
            this._context = context;
            this._configure = configure;
        }

        /// <summary>
        /// Create a new JWT token for a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="hasRole"></param>
        /// <returns></returns>
        public string CreateToken(User user, string hasRole = null)
        {
            // Set up a security key based on the key in appsettings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configure["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create new refreshtoken
            var newRefreshToken = CreateRefreshToken(user);

            // Set token claims payload
            var claims = new List<Claim>()
            {
                    //new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("Name", user.FirstName + " " + user.LastName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserEmail", user.Email),
                    new Claim("RefreshToken", newRefreshToken)
            };

            // If user has a role, add it to list of claims
            if (hasRole != null)
                claims.Add(new Claim(ClaimTypes.Role, hasRole));

            // Set token settings
            var token = new JwtSecurityToken(
                issuer: _configure["JWT:Issuer"],
                audience: _configure["JWT:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddSeconds(30),
                signingCredentials: credentials
            );

            // Create token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Create and stores a new refresh token in database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateRefreshToken(User user)
        {
            user.RefreshToken = Guid.NewGuid().ToString();
            user.RefreshTokenExpire = DateTime.UtcNow.AddMonths(6);

            _context.Update<User>(user);
            _context.SaveChanges();

            // return RefreshToken
            return user.RefreshToken;
        }

    }
}
