using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }
        [Required]
        public int AdmittedPatientID { get; set; }
        
        [Required]
        public int AccountID { get; set; }
        [Required]
        public DateOnly DateGiven { get; set; }

        [Required]
        public string Urgency { get; set; }
       

        [Required]
        public string Status { get; set; }
       

    }
}
