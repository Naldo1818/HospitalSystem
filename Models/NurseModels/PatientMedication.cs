using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientMedication
    {
        [Key]
        public int PatientMedicationID { get; set; }
        [Required]  
        public string PatientID { get; set; }
        [Required]
        public string CurrentID { get; set; }
        
    }
}
