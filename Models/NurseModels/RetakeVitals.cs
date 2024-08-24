using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class RetakeVitals
    {
        
            [Key]
            public int RetakeVitalsID { get; set; }

            [Required]
            public new int AdmittedPatientID { get; set; }  // Ensure this is still required in this model

            [Required]
            public new int SystolicBloodPressure { get; set; }

            [Required]
            public new int DiastolicBloodPressure { get; set; }

            [Required]
            public new int HeartRate { get; set; }

            [Required]
            public new int BloodOxygen { get; set; }

            [Required]
            public new int Respiration { get; set; }

            [Required]
            public new int BloodGlucoseLevel { get; set; }

            [Required]
            public new int Temperature { get; set; }

            [Required]
            public DateTime RetakeDate { get; set; }
        
    }
}

