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
        public int AdmissionId { get; set; }
        
        [Required]
        [ForeignKey("PatientID")]
        public int PatientID { get; set; }

        [Required]
        [ForeignKey("WardID")]
        public int WardID { get; set; }

        [Required]
        public int PatientAllergyID { get; set; }

        [Required]
        public int PatientConditionsID { get; set; }

        [Required]
        public int PatientMedicationID { get; set; }

        [Required]
        public DateTime Date = DateTime.Now;
        [Key]
        public int ProvinceID { get; set; }
        [Key]
        public int CityID { get; set; }
        [Key]
        public int SuburbID { get; set; }
        


        [Required]
        public string AdmissionStatus { get; set; }

        public BookSurgery bookSurgery { get; set; }
        public List<BookSurgery> bookings { get; set; }
    }
}
