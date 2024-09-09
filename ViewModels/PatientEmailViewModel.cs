using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientEmailViewModel
    {
        public DateOnly SurgeryDate { get; set; }
        public string SurgeryTime { get; set; }

        [StringLength(200, ErrorMessage = "Full Name cannot be longer than 200 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name can only contain letters and spaces.")]
        public string FullName { get; set; }
        public string AccountName { get; set; }

        public string AccountSurname { get; set; }
        public int BookingID { get; set; }
        public string BookingTreatmentCodeID { get; set; }
        public string TreatmentName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Theater cannot be longer than 100 characters.")]
        public string Theater { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid contact number format.")]
        public string ContactNumber { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot be longer than 500 characters.")]
        public string Notes { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
