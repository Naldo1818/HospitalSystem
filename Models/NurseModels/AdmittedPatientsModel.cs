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

        public int AddressID { get; set; }
       
        [Required]
        public DateTime Date = DateTime.Now;
        [Required]
        public int AdmissionStatusID { get; set; }
        [Required]
        public TimeOnly Time = TimeOnly.FromDateTime(DateTime.Now);
       
    }
}
 