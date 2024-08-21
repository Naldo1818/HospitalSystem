using DEMO.Models;

namespace DEMO.ViewModels
{
    public class PrescriptionMedicationViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Instructions { get; set; }
        public int Quantity { get; set; }
        public DateOnly DateGiven { get; set; }
        public string Status { get; set; }
        public string MedicationName { get; set; }
        public string MedicationForm { get; set; }
        public int PrescriptionID { get; set; }
        public List<Medication> AllMedication { get; set; }
        public List<PrescriptionMedicationViewModel> CombinedData { get; set; }
    }
}
