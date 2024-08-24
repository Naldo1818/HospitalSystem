using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class SurgeonViewModel
    {
        [Required]
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(254, ErrorMessage = "Email cannot be longer than 254 characters.")]
        public string Email { get; set; }
    }
}
