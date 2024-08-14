using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Medication
    {
           //Move Quantity and Instructions
        [Key]
        public int MedicationID { get; set; }

        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string MedicationForm { get; set; }
                
        [Required]
        public int Schedule { get; set; }
        
        

    }
}
