using DEMO.Models;

namespace DEMO.ViewModels
{
    public class EditTreatmentListViewModel
    {
        public int BookingID { get; set; }
        public int TreatmentCodeID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TreatmentName { get; set; }
        public string TreatmentCode { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string SurgeryTime { get; set; }
        public IEnumerable<TreatmentCodes> AllTreatmentCodes { get; set; }
        public List<EditTreatmentListViewModel> AllcombinedData { get; set; }
    }
}
