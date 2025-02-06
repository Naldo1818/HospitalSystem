using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PrescriptionListViewModal
    {

        [StringLength(500, ErrorMessage = "Rejection Reason cannot be longer than 500 characters.")]
        public string RejectionReason { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Patient Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Patient Name can only contain letters and spaces.")]
        public string PatientName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Patient Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Patient Surname can only contain letters and spaces.")]
        public string PatientSurname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Account Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Account Name can only contain letters and spaces.")]
        public string AccountName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Account Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Account Surname can only contain letters and spaces.")]
        public string AccountSurname { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Urgency cannot be longer than 50 characters.")]
        public string Urgency { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]
        public string Status { get; set; }

        [Required]
        public int PrescriptionID { get; set; }
        
      
        public DateOnly DateGiven { get; set; }
        public List<PrescriptionListViewModal> AllPrescribedRejected { get; set; }
        public List<PrescriptionListViewModal> AllPrescribedDispensed { get; set; }
        public List<PrescriptionListViewModal> AllPrescribed { get; set; }
        public string? MedicationName { get;  set; }
        public string? Instructions { get;  set; }
        public int Quantity { get;  set; }
        public int MedicationID { get; set; }
    }
}
