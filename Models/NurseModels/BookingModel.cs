using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class BookingModel
    {
        [Key]
        public int BookingID { get; set; }
        [Required]
        public int AccountID { get; set; }
        [Required]
        public int PatientID { get; set; }

        [Required]
        public string SurgeryTime { get; set; }

        [Required]
        public DateOnly SurgeryDate { get; set; }

        [Required]
        public string Theater { get; set; }

        
    }
}
