using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class UpdateUserPasswordModel
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
