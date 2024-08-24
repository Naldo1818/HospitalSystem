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
        
        public int WardID { get; set; }

        [Required]
        public int PatientAllergyID { get; set; }

        [Required]
        public int PatientConditionsID { get; set; }

        [Required]
        public int PatientMedicationID { get; set; }

        public int PatientVitalsID { get; set; }

        [Required]
        public DateTime Date = DateTime.Now;
        
        public int ProvinceID { get; set; }
      
        public int CityID { get; set; }
     
        public int SuburbID { get; set; }
        [Required]
        public string StreetName {  get; set; }

        [Required]
        public string AdmissionStatus { get; set; } = "Admitted";
        public int SurgeryBookinID { get; set; }
    }
}
