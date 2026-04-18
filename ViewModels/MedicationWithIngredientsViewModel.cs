using DEMO.Models;

namespace DEMO.ViewModels
{
    public class MedicationWithIngredientsViewModel
    {
        public string MedicationName { get; set; }
        public string MedicationForm { get; set; }
        public int Schedule { get; set; }
        public int StockonHand { get; set; }
        public int ReorderLevel { get; set; }

        public List<MedicationActiveIngredient> Ingredients { get; set; }
        public List<Medication> AllMedication { get; set; }
        public List<Activeingredient> ActiveIngredients { get; set; }
    }
}
