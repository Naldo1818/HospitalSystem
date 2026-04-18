using DEMO.Models;
using DEMO.Models.NurseModels;

namespace DEMO.ViewModels
{
    public class PatientAllergiesConditionsViewModel
    {
        public List<Activeingredient> AllActiveIngredients { get; set; } = new();
        public List<Condition> AllConditions { get; set; } = new();

        // For tables (clean display)
        public List<PatientAllergyVM> PatientAllergieslist { get; set; } = new();
        public List<PatientConditionVM> PatientConditionslist { get; set; } = new();
    }

    public class PatientAllergyVM
    {
        public int patientAllergyID { get; set; }
        public string Allergy { get; set; }
        public int PatientID { get; set; }
    }

    public class PatientConditionVM
    {
        public int patientConditionsID { get; set; }
        public string Condition { get; set; }
        public int PatientID { get; set; }
    }
}