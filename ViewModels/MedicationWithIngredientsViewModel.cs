using DEMO.Models;

namespace DEMO.ViewModels
{
    public class MedicationWithIngredientsViewModel
    { public int ActiveingredientID { get; set; }
        public int ActiveIngredientStrength { get; set; }
        public string ActiveIngredientName { get; set; }
        public string MedicationName { get; set; }
        public string MedicationForm { get; set; }
        public int Schedule { get; set; }
        public int StockonHand { get; set; }
        public int ReorderLevel { get; set; }

        // ✅ FIXED: matches JS + controller
        public List<IngredientItem> Ingredients { get; set; }

        // For display
        public List<Medication> AllMedication { get; set; }
        public List<Activeingredient> ActiveIngredients { get; set; }
        public List<MedicationWithIngredientsViewModel> AllMedicationActiveIngredients { get; set; }
    }

    public class IngredientItem
    {
        public string ActiveIngredientName { get; set; }
        public int ActiveingredientID { get; set; }
        public int ActiveIngredientStrength { get; set; }
    }
}