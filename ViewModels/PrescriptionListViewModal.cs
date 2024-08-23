namespace DEMO.ViewModels
{
    public class PrescriptionListViewModal
    {
        public string IDNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Urgency { get; set; }
        public string Status { get; set; }
        public string Take { get; set; }
        public DateOnly DateGiven { get; set; }
        public List<PrescriptionListViewModal> AllPrescribed { get; set; }
    }
}
