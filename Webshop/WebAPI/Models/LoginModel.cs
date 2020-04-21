using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class LoginModel
    {
        [Required]
        [RegularExpression(@"^[a-z\d._%+-]+@[a-z\d.-]+\.[a-z]{2,}$")]
        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public bool RememberUser { get; set; }
    }
}
