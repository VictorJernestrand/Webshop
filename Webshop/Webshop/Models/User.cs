using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Fyll i Förnamn")]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Ange din e-postadress")]
        public string Email { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public int ZipCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required(ErrorMessage = "Ange ditt lösenord")]
        public string Password { get; set; }
        public List<Order> Orders { get; set; }

    }
}
