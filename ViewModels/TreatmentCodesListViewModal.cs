using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class TreatmentCodesListViewModal
    {//TreatmentCodes

        [Required(ErrorMessage = "Treatment Name is required.")]
        [StringLength(100, ErrorMessage = "Treatment Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Treatment Name can only contain letters and spaces.")]
        public string TreatmentName { get; set; }

        [Required(ErrorMessage = "Treatment Code is required.")]
        [StringLength(20, ErrorMessage = "Treatment Code cannot be longer than 20 characters.")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Treatment Code can only contain uppercase letters and numbers.")]
        public string TreatmentCode { get; set; }

        //List
        public TreatmentCodes treatmentCodes { get; set; }
        public List<TreatmentCodes> AllTreatmentCodes { get; set; }
    }
}
