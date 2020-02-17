using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Ange e-postadress")]
        [EmailAddress]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Ange lösenord")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public bool RememberUser { get; set; }
    }
}
