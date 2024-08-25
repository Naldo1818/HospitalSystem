using DEMO.ViewModels;


namespace DEMO.Models.NurseModels
{
    public class ViewBookings
    {
        public int BookingID { get; set; }
        public string AccountName { get; set; }
        public string AccountSurname { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public int PatientID { get; set; }
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string Theater { get; set; }
        public List<ViewBookings> AllcombinedData { get; set; }
        
    }
}
