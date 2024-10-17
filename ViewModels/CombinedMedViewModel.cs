using DEMO.Models;
using DEMO.ViewModels;
using DEMO.Models.PharmacistModels;



namespace DEMO.ViewModels
{
    public class CombinedMedViewModel
    { 
        public PharmacyMedicationModel PharmacyMedication { get; set; }

       public AddMedicationViewModel AddMedication { get; set; }

    }
}