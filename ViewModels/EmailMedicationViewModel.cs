using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class EmailMedicationViewModel
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public int Amount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
