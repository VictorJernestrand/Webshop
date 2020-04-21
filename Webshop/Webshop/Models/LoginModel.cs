using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Ange e-postadress")]
        [EmailAddress]
        [RegularExpression(@"^[a-z\d._%+-]+@[a-z\d.-]+\.[a-z]{2,}$", ErrorMessage = "E-postadressen är ogiltig")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Ange lösenord")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public bool RememberUser { get; set; }
    }
}
