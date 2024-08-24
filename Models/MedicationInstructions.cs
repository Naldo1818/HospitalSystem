using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class MedicationInstructions
    {
        [Key]
        public int InstructionsID { get; set; }
       
        [Required]
        public int PrescriptionID { get; set; }

        [Required]
        public int MedicationID { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Instructions cannot be longer than 500 characters.")]
        public string Instructions { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }


    }
}
