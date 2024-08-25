using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class CurrentMedication
    {
        [Key]
        public int CurrentId { get; set; }
        
        [Required]
        public string MedicationName { get; set; }

       
    }
}
