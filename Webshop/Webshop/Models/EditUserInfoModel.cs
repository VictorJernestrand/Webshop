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
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Namn kan inte vara tomt")]
        [MaxLength(30)]

        public string LastName { get; set; }

        [Required(ErrorMessage = "Telefonnummer måste fyllas i")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "E-post måste finnas")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Adress kan inte vara tomt")]
        [MaxLength(50)]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "PostNr måste finnas")]
        [DataType(DataType.PostalCode)]
        public int ZipCode { get; set; }

        [Required(ErrorMessage = "Stad kan inte vara tomt")]
        [MaxLength(30)]
        public string City { get; set; }

    }
}
