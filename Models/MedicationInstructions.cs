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
        public string Instructions { get; set; }

        [Required]
        public int Quantity { get; set; }


    }
}
