using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Medication
    {
         
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
