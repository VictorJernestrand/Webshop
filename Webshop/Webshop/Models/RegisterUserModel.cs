using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Förnamn saknas")]
        [StringLength(30)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Efternamn saknas")]
        [StringLength(30)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Felaktig e-postadress")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Välj ett lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Välj ett lösenord")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Lösenorden matchar inte!")]
        public string PasswordConfirm { get; set; }

    }
}
