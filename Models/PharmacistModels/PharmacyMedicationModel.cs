using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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


        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }

        public List<string> PharmMedDF { get; set; } = new List<string>(); // Initialize here

        public List<int> PharmMedSchedule { get; set; } = new List<int>(); // Initialize here

        //public List<string> ActiveIngredients { get; set; } = new List<string>();








      


        //public string IngredientandStrength { get; set; }


        //public List<string> IngredientsplusStrength { get; set; }

        //public string Ingredient { get; set; }

        //public string Strength { get; set; }






    }
}
