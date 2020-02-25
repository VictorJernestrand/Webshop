using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class EditUserInfoModel
    {
        [Required(ErrorMessage = "Namn kan inte vara tomt")]
        [MaxLength(30)]
        [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Förnamnet innehåller ogiltiga tecken")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Namn kan inte vara tomt")]
        [MaxLength(30)]
        [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Efternamnet innehåller ogiltiga tecken")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Telefonnummer måste fyllas i")]
        [RegularExpression(@"^\+\d{2}-\d{2}-\d{3}\s?\d{2}\s?\d{2}|\d{2,3}-\d{3}\s?\d{2}\s?\d{2}$", ErrorMessage = "Telefonnumret är ogiltigt")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "E-post måste finnas")]
        [RegularExpression(@"^[a-z\d._%+-]+@[a-z\d.-]+\.[a-z]{2,}$", ErrorMessage = "E-postadressen är ogiltig")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress kan inte vara tomt")]
        [MaxLength(50)]
        [RegularExpression(@"^\p{L}{6,} \d+$", ErrorMessage = "Felaktigt inmatning")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "PostNr måste finnas")]
        [DataType(DataType.PostalCode)]
        [RegularExpression(@"^\d{3}\s?\d{2}$", ErrorMessage = "Postnummret är ogiltigt")]
        public int ZipCode { get; set; }

        [Required(ErrorMessage = "Stad kan inte vara tomt")]
        [MaxLength(30)]
        [RegularExpression(@"^\p{L}+[ \s]?\p{L}+$", ErrorMessage = "Ogiltiga tecken")]
        public string City { get; set; }

    }
}
