using DEMO.Models.NurseModels;

namespace DEMO.ViewModels
{
    public class ViewBookings
    {
        public int BookingID { get; set; }
        public int AccountID { get; set; }
        public int WardID { get; set; }
        public int BedId { get; set; }
        public string ContactNumber { get; set; }
        public string AccountName { get; set; }
        public string AccountSurname { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public int PatientID { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string SurgeryTime { get; set; }
        public string Theater { get; set; }
        public bool IsAdmitted { get; set; }
        public int ProvinceID { get; set; }
        public int CityID { get; set; }
        public int SuburbID { get; set; }
        public string StreetName { get; set; }

        // ✅ ADD THESE
        public List<ViewBookings> AllcombinedData { get; set; }
        public List<Ward> AllWards { get; set; }
        public List<Suburb> AllSuburb { get; set; }
        public List<Province> AllProvince { get; set; }
        public List<City> AllCity { get; set; }
        public List<Bed> AllBed { get; set; }
    }
}
