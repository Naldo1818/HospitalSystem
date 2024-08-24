using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PatientInfo
{
    [Key]
    public int PatientID { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
    public string Name { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
    public string Surname { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "Gender must be specified.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Gender can only contain letters and spaces.")]
    public string Gender { get; set; }

    [Required]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string ContactNumber { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; }

    [Required]
    [StringLength(20, ErrorMessage = "ID Number cannot be longer than 20 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "ID Number can only contain letters and numbers.")]
    public string IDNumber { get; set; }
}
