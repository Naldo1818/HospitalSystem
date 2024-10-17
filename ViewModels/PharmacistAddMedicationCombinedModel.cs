using DEMO.Models.PharmacistModels;
using DEMO.ViewModels;

namespace DEMO.ViewModels
{
    public class PharmacistAddMedicationCombinedModel
    {

        public AddMedicationViewModel addMedication { get; set; }
         public PharmacyMedicationModel pharmacy { get; set; }


        public IEnumerable<AddMedicationViewModel> Medications { get; set; }

        public IEnumerable<PharmacyMedicationModel> PharmacyMedications { get; set; }

    }
}
