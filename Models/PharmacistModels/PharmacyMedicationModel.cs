using System.ComponentModel.DataAnnotations;
using DEMO.Models;
using DEMO.ViewModels;

namespace DEMO.Models.PharmacistModels
{
    public class PharmacyMedicationModel
    {
        [Key]
        public int PharmacyMedicationID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Medication Name can only contain letters, numbers, and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Dosage form can only contain letters, numbers, and spaces.")]
        public string DosageForm { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Schedule must be between 0 and 6.")]
        public int Schedule { get; set; }

        public List<string> PharmacyMedications { get; set; } // Assuming these are strings


        public List<string> PharmMedDF { get; set; } = new List<string>(); // Initialize here

        public List<int> PharmMedSchedule { get; set; } = new List<int>(); // Initialize here

        [Required]
        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }


        //public string IngredientandStrength { get; set; }


        //public List<string> IngredientsplusStrength { get; set; }



    }
}
