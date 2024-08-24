using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PrescriptionMedicationViewModel
    {
        public int Medid { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Instructions cannot be longer than 500 characters.")]
        public string Instructions { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }
        public DateOnly DateGiven { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]
        public string Status { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Schedule must be between 0 and 6.")]
        public int Schedule { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Medication Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Medication Name can only contain letters and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Medication Form cannot be longer than 50 characters.")]
        public string MedicationForm { get; set; }
        public int PrescriptionID { get; set; }
        public List<Medication> AllMedication { get; set; }
        public List<PrescriptionMedicationViewModel> CombinedData { get; set; }
    }
}
