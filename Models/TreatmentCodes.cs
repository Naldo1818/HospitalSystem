using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class TreatmentCodes
    {
        [Key]
        public int TreatmentCodeID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Treatment Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Treatment Name can only contain letters and spaces.")]
        public string TreatmentName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Treatment Code cannot be longer than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Treatment Code can only contain letters and numbers.")]
        public string TreatmentCode { get; set; }


    }
}
