using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Data
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StreetAddress { get; set; }

        public int ZipCode { get; set; }

        public string City { get; set; }

        public Guid RefreshToken { get; set; }

        public DateTime RefreshTokenExpire { get; set; }

        [NotMapped]
        public string Password { get; set; }

        public List<Order> Orders { get; set; }

    }
}
