using System.ComponentModel.DataAnnotations;
using DEMO.Models;
using DEMO.Models.PharmacistModels;

namespace DEMO.Models
{
    public class MedicationActiveIngredient
    {
        [Key]
        public int MedicationActiveingredientID { get; set; }
        [Required]
        public int MedicationID { get; set; }
        [Required]
        public int ActiveingredientID { get; set; }
        [Required]
        [Range(1, 1000, ErrorMessage = "Active Ingredient Strength must be between 1 and 1000.")]
        public int ActiveIngredientStrength { get; set; }
       
    }
}
