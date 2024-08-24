using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class AccountsListViewModal
    {//RegisterPatient
        [Required]
        public int AccountID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Username can only contain letters and numbers.")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters.")]
        public string Password { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid contact number format.")]
        public string ContactNumber { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Registration Number must be a positive number.")]
        public int RegistrationNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Status can only contain letters and spaces.")]
        public string Status { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Role cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Role can only contain letters and spaces.")]
        public string Role { get; set; }



        //List
        public Accounts accounts { get; set; }
        public List<Accounts> AllAccounts { get; set; }
    }
}

