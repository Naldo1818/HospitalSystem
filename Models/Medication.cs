using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Medication
    {
         
        [Key]
        public int MedicationID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Medication Name can only contain letters, numbers, and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Medication form can only contain letters, numbers, and spaces.")]
        public string MedicationForm { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Schedule must be between 0 and 6.")]
        public int Schedule { get; set; }
        
        

    }
}
