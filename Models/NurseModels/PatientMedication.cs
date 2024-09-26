using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientMedication
    {
        [Key]
        public int PatientMedicationID { get; set; }
        [Required]  
        public int PatientID { get; set; }
        
        [Required]
        public int MedicationID { get; set; }
        
    }
}
