using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class EditTreatmentListViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "TreatmentName cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "TreatmentName can only contain letters and spaces.")]
        public string TreatmentName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "TreatmentCode cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "TreatmentCode can only contain letters and numbers.")]
        public string TreatmentCode { get; set; }

        [Required]
        public DateOnly SurgeryDate { get; set; }

        [Required]
        public string SurgeryTime { get; set; }
        public IEnumerable<TreatmentCodes> AllTreatmentCodes { get; set; }
        public List<EditTreatmentListViewModel> AllcombinedData { get; set; }
    }
}
