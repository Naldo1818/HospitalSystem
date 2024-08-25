using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class ViewAdmittedPatients
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
        public int PatientDetailsID { get; set; }

        [Required]
        public DateTime Date = DateTime.Now;
       

        [Required]
        public string AdmissionStatus { get; set; } = "Admitted";
        [Required]
        public TimeOnly Time = TimeOnly.FromDateTime(DateTime.Now);
        public List<ViewAdmittedPatients> VAP { get; set; }
    }
}
