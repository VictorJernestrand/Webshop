using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class CreditCardModel
    {
        [Required(ErrorMessage = "Ange kortnummer")]
        [RegularExpression(@"^[0-9]{16}?$", ErrorMessage = "Kortnumret är ogiltigt")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Ange CVC-numret")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "CVC-numret är ogiltigt")]
        public string CVC { get; set; }
    }
}
