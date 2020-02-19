using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class UpdateUserPassword
    {
        [Required(ErrorMessage = "Nuvarande Lösenord")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Lösenord kan inte vara tomt")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Lösenord kan inte vara tomt")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Lösenorden matchar inte!")]
        public string NewPasswordConfirm { get; set; }
    }
}
