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
        [Key]
        public int PatientID { get; set; }

        [Required]
        [Key]
        public int WardID { get; set; }

        [Required]
        public List<PatientAllergy> PatientAllergyID { get; set; }

        [Required]
        public List<PatientConditions> PatientConditionsID { get; set; }

        [Required]
        public List<PatientMedication> PatientMedication { get; set; }

        public int PatientVitalsID { get; set; }

        [Required]
        public DateTime Date = DateTime.Now;
        [Key]
        public int ProvinceID { get; set; }
        [Key]
        public int CityID { get; set; }
        [Key]
        public int SuburbID { get; set; }
        [Required]
        public string StreetName {  get; set; }

        [Required]
        public string AdmissionStatus { get; set; } = "Admitted";
        public List<BookSurgery> bookings { get; set; }
    }
}
