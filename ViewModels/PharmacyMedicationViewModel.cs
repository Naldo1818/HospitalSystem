using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PharmacyMedicationViewModel
    {
        [Key]
        public int PharmacyMedicationID { get; set; }
        [Required]
        public int MedicationID { get; set; }

        [Required]
        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }

        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string MedicationForm { get; set; }
        [Required]
        public int Schedule { get; set; }
    }
}
