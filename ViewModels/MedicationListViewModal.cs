using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
   
    public class MedicationListViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Medication Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Medication Name can only contain letters and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Active Ingredient Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Active Ingredient Name can only contain letters and spaces.")]
        public string ActiveIngredientName { get; set; }
        public int ActiveIngredientStrength { get; set; }
      
        public List<MedicationListViewModel> AllMedicationActiveIngredients { get; set; }

      
        public List<Medication> AllMedication { get; set; }
        public IEnumerable<Activeingredient> ActiveIngredients { get; set; }
    }
}
