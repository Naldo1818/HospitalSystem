using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class PatientInfo
    {
        [Key]
        public int PatientID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string IDNumber { get; set; }

      


    }
}
