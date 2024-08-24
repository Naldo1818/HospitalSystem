using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientListViewModal
    {
        //RegisterPatient
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID Number must be exactly 13 characters.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ID Number must be exactly 13 digits.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender cannot be longer than 10 characters.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(254, ErrorMessage = "Email cannot be longer than 254 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid contact number.")]
        [StringLength(15, ErrorMessage = "Contact number cannot be longer than 15 characters.")]
        public string ContactNumber { get; set; }

        // BookSurgery
        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountID { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }
        [Required]
        public string SurgeryTime { get; set; }
        [Required]
        public DateOnly SurgeryDate { get; set; }
        [Required]
        public string Theater { get; set; }




        //List
        public PatientInfo patientInfo { get; set; }
        public List<PatientInfo> AllPatients { get; set; }
    }
}
