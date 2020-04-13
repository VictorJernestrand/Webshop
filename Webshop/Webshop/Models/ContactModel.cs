using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Ange e-postadress")]
        [EmailAddress]
      [RegularExpression(@"^[a-z\d._%+-]+@[a-z\d.-]+\.[a-z]{2,}$", ErrorMessage = "E-postadressen är ogiltig")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Namn kan inte vara tomt")]
        [MaxLength(30)]
      //[RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Förnamnet innehåller ogiltiga tecken")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Namn kan inte vara tomt")]
        [MaxLength(30)]
      // [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Efternamnet innehåller ogiltiga tecken")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Meddelandet kan inte vara tomt")]
        [MaxLength(1000)]
        public string Message { get; set; }

        [MaxLength(10)]
        //[RegularExpression(@"^\+\d{2}-\d{2}-\d{3}\s?\d{2}\s?\d{2}|\d{2,3}-\d{3}\s?\d{2}\s?\d{2}$", ErrorMessage = "Telefonnumret är ogiltigt")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}
