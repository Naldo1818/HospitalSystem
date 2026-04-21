using DEMO.Models;
using DEMO.Models.PharmacistModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{

    public class MedicationOrderViewModel
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public int Amount { get; set; }
        public DateTime OrderDate { get; set; }

        public List<MedicationOrderViewModel> Orders { get; set; }
        public List<Medication> AllMedications { get; set; }

        public int? SelectedMedicationId { get; set; }
    }
}
