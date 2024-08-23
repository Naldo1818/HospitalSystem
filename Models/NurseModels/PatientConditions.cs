using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientConditions
    {
        [Key]
        public int PatientConditionsID { get; set; }
        [Required]
        public string PatientID { get; set; }
        [Required]
        public string ConditionsID { get; set; }
    }
}
