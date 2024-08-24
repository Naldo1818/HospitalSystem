using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class AdmissionsListViewModel
    {
        public int PatientID { get; set; }
        public int BookingID { get; set; }
        public int AccountID { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string Theater { get; set; }
        public List<AdmissionsListViewModel> AllcombinedData { get; set; }
    }
}
