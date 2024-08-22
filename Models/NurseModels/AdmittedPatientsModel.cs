using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class AdmittedPatientsModel
    {
        [Key]
        public int AdmissionId { get; set; }

        [Required]
        [ForeignKey("BookingID")]
        public int BookingID { get; set; }

        [Required]
        [ForeignKey("PatientID")]
        public int PatientID { get; set; }

        [Required]
        [ForeignKey("WardID")]
        public int WardID { get; set; }

        [Required]
        public DateTime Date = DateTime.Now;

        [Required]
        public string AdmissionStatus { get; set; }

        public BookSurgery bookSurgery { get; set; }
        public List<BookingModel> bookings { get; set; }
    }
}
