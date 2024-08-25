using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientAllergyViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ActiveIngredientName { get; set; }
        public string ConditionName { get; set; }
        public List<PatientAllergyViewModel> Allallergy { get; set; }
        public List<PatientAllergyViewModel> AllCurrentMed { get; set; }
        public List<PatientAllergyViewModel> AllConditions { get; set; }
    }
}
