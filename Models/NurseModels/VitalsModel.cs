using Org.BouncyCastle.Bcpg.Sig;
using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class VitalsModel
    {
        [Key]
        public int VitalsID { get; set; }
        [Required]
        
        [RegularExpression("")]
        public int AdmittedPatientID { get; set; }
        [Required]
        public int Height { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public int SystolicBloodPressure { get; set; }
        [Required]

        public int DiastolicBloodPressure { get; set; }
        [Required]
        public int HeartRate { get; set; }

        [Required]
        public int BloodOxygen { get; set; }
        [Required]
        public int Respiration { get; set; }
        [Required]
        public int BloodGlucoseLevel { get; set; }
        [Required]
        public int Temperature { get; set; }
        [Required]
        public TimeOnly TimeOnly { get; set; }
    }
}
