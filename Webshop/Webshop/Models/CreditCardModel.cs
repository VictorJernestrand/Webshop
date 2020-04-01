using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class CreditCardModel
    {
        [Required(ErrorMessage = "Ange kortnummer")]
        [RegularExpression(@"^[0-9]{16}?$", ErrorMessage = "Kortnumret är ogiltigt")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Ange CVC-numret")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "CVC-numret är ogiltigt")]
        public int CVC { get; set; }

    }
}
