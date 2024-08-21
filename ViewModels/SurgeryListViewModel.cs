namespace DEMO.ViewModels
{
    public class SurgeryListViewModel
    {
        public int PatientID { get; set; }
        public int BookingID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
       
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string Theater { get; set; }
        public List<SurgeryListViewModel> AllcombinedData { get; set; }
     
    }
}
