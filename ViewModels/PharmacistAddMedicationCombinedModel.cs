using DEMO.Models.PharmacistModels;
using DEMO.ViewModels;

namespace DEMO.ViewModels
{
    public class PharmacistAddMedicationCombinedModel
    {

        public AddMedicationViewModel addMedication { get; set; }
        public IEnumerable<AddMedicationViewModel> Medications { get; set; }


    }
}
