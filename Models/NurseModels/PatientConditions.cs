using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientConditions
    {
        [Key]
        public int PatientConditionsID { get; set; }
        [Required]
        public int PatientID { get; set; }
        [Required]
        public int AdmittedPatientID { get; set; }
        [Required]
        public int ConditionsID { get; set; }
    }
}
