using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class BookSurgery
    {
        [Key]
        public int BookingID { get; set; }
        [Required]
        public int SurgeonID { get; set; }
        [Required]
        public int PatientID { get; set; }

        [Required]
        public TimeOnly SurgeryTime { get; set; }

        [Required]
        public DateOnly SurgeryDate { get; set; }

        [Required]
        public string Theater { get; set; }
        [Required]
        public string AdmissionStatus { get; set; }
    }
}
