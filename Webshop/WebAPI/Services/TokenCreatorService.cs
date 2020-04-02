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
        public APIPayload CreateToken(User user, bool isAdmin, bool generateRefreshToken = false)
        {
            // Set up a security key based on the key in appsettings.json
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configure["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Generate a new refresh token
            string newRefreshToken = null;
            if (generateRefreshToken)
                newRefreshToken = CreateRefreshToken(user).ToString();

            // Create new JWT-token
            var claims          = SetTokenClaims(user, isAdmin);
            var token           = ConfigureToken(claims, credentials);

            APIPayload tokenPayload = new APIPayload()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = newRefreshToken
            };

            return tokenPayload;
        }

        /// <summary>
        /// Create and stores a new refresh token in database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private Guid CreateRefreshToken(User user)
        {
            user.RefreshToken = Guid.NewGuid();
            user.RefreshTokenExpire = DateTime.UtcNow.AddMonths(6);

            _context.Update<User>(user);
            _context.SaveChanges();

            // return RefreshToken
            return user.RefreshToken;
        }


        private IEnumerable<Claim> SetTokenClaims(User user, bool isAdmin)
        {
            // Set token claims payload
            var claims = new List<Claim>()
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Name", user.FirstName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserEmail", user.Email),
                new Claim(ClaimTypes.Role, (isAdmin) ? "Admin" : "User")
            };

            return claims;
        }

        private JwtSecurityToken ConfigureToken(IEnumerable<Claim> claims, SigningCredentials credentials)
        {
            var token = new JwtSecurityToken(
                issuer: _configure["JWT:Issuer"],
                audience: _configure["JWT:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddSeconds(int.Parse(_configure["JWT:TokenExpireSeconds"])),
                signingCredentials: credentials
            );

            return token;
        }

    }
}
