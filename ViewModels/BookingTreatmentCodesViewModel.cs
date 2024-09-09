using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class BookingTreatmentCodesViewModel
    {
        public int btcID { get; set; }
        public int BookingID { get; set; }

        public int TreatmentCodeID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Treatment Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Treatment Name can only contain letters and spaces.")]
        public string TreatmentName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Treatment Code cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Treatment Code can only contain letters and numbers.")]
        public string TreatmentCode { get; set; }
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public string Theater { get; set; }
        
        public List<BookingTreatmentCodesViewModel> AllcombinedData { get; set; }
        public List<SurgeryTreatmentCode> AllSurgeryTreatmentCode { get; set; }
        public IEnumerable<TreatmentCodes> AllTreatmentCodes { get; set; }
    }
}
