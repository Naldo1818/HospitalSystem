using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Password must be at least 10 characters long.")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Password must not contain spaces.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

