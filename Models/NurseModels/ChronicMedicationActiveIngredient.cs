using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class ChronicMedicationActiveIngredient
    {
        [Key]
        public int CMedicationActiveingredientID { get; set; }
        [Required]
        public int CMedicationID { get; set; }
        [Required]
        public int ActiveingredientID { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Active Ingredient Strength must be between 1 and 1000.")]
        public int ActiveIngredientStrength { get; set; }

    }
}
