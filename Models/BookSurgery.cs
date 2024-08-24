using System;
using System.ComponentModel.DataAnnotations;
namespace DEMO.Models
{
    public class BookSurgery
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
        [StringLength(100, ErrorMessage = "Theater name can't be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "The Theater name can only contain letters, numbers, and spaces.")]
        public string Theater { get; set; }
       
    }
}
