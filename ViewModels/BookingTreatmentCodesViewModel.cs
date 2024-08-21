using DEMO.Models;

namespace DEMO.ViewModels
{
    public class BookingTreatmentCodesViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TreatmentName { get; set; }
        public string TreatmentCode { get; set; }
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string Theater { get; set; }
        public int BookingID { get; set; }
        public List<BookingTreatmentCodesViewModel> AllcombinedData { get; set; }
        public List<SurgeryTreatmentCode> AllSurgeryTreatmentCode { get; set; }
        public IEnumerable<TreatmentCodes> AllTreatmentCodes { get; set; }
    }
}
