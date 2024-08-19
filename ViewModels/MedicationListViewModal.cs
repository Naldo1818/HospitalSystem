using DEMO.Models;

namespace DEMO.ViewModels
{
   
    public class MedicationListViewModel
    {
        public string MedicationName { get; set; }
        public string ActiveIngredientName { get; set; }
        public int ActiveIngredientStrength { get; set; }
      
        public List<MedicationListViewModel> AllMedicationActiveIngredients { get; set; }

      
        public List<Medication> AllMedication { get; set; }
        public IEnumerable<Activeingredient> ActiveIngredients { get; set; }
    }
}
