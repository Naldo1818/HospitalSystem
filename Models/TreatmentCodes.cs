using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class TreatmentCodes
    {
        [Required]
        public int TreatmentCodeID { get; set; }
        [Required]
        public int PrescriptionID { get; set; }
        
        [Required]
        public string TreatmentName { get; set; }

        [Required]
        public int TreatmentCode { get; set; }

       
    }
}
