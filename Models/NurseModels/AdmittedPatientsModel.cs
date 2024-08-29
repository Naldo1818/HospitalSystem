using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Crypto.Utilities;

namespace DEMO.Models.NurseModels
{
    public class AdmittedPatientsModel
    {
        [Key]
        public int AdmittedPatientID { get; set; }
        
        [Required]
    
        public int PatientID { get; set; }
        [Required]

        public int BookingID { get; set; }

        [Required]
      
        public int WardID { get; set; }


        [Required]

        public int PatientVitalsID { get; set; }
        [Required]

        public int PatientConditionsID { get; set; }
        [Required]

        public int PatientMedicationID { get; set; }

        [Required]

        public int PatientAllergyID { get; set; }

        public int PatientDetailsID {  get; set; }

        [Required]
        public DateTime Date = DateTime.Now;

        [Required]
        public AdmissionStatus AdmissionStatus { get; set; }
        [Required]
        public TimeOnly Time = TimeOnly.FromDateTime(DateTime.Now);
       
    }
}
 