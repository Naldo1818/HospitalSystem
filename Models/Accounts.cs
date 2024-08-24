using System.ComponentModel.DataAnnotations;

public class Accounts
{
    [Key]
    public int AccountID { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^\S*$", ErrorMessage = "Password must not contain spaces.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Contact Number is required.")]
    [Phone(ErrorMessage = "Invalid Contact Number format.")]
    public string ContactNumber { get; set; }

    [Required(ErrorMessage = "Registration Number is required.")]
    public int RegistrationNumber { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; }
}
