using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class SurgeryTreatmentCode
    {
        [Key]
        public int BookingTreatmentCodeID { get; set; }
        [Required]
        public int BookingID { get; set; }
        [Required]
        public int TreatmentCodeID { get; set; }
    }
}
