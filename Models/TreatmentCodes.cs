using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class TreatmentCodes
    {
        [Key]
        public int TreatmentCodeID { get; set; }
        [Required]
        public int BookingID { get; set; }
        
        [Required]
        public string TreatmentName { get; set; }

        [Required]
        public int TreatmentCode { get; set; }

       
    }
}
