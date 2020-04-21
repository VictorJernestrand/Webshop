using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "Förnamn saknas")]
        [StringLength(30)]
        [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Förnamnet innehåller ogiltiga tecken")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Efternamn saknas")]
        [StringLength(30)]
        [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Efternamnet innehåller ogiltiga tecken")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-postadress saknas")]
        [RegularExpression(@"^[a-z\d._%+-]+@[a-z\d.-]+\.[a-z]{2,}$", ErrorMessage = "E-postadressen är ogiltig")]
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
