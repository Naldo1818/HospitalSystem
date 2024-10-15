namespace DEMO.ViewModels
{
    public class ReportViewModel
    {
        public string Patient { get; set; }
        public string TreatmentName { get; set; }
        public string TreatmentCode { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public List<ReportViewModel> AllcombinedData { get; set; }
    }
}
