using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientVitals
    {
        [Key]
        public int PatientVitalsID { get; set; }
        [Key]
        public int VitalsID { get; set; }
        [Key]
        public int AdmittedPatientID { get; set; }
        [Required]
        public int Value { get; set; }
    }
}
