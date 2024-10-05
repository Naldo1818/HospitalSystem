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

        public int AddressID { get; set; }

        public int BedId { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public int AdmissionStatusID { get; set; }

        [Required]
        public TimeOnly Time { get; set; }

        // Constructor to set default values
        public AdmittedPatientsModel()
        {
            Date = DateOnly.FromDateTime(DateTime.Now);
            Time = TimeOnly.FromDateTime(DateTime.Now);
        }
    }
}
 