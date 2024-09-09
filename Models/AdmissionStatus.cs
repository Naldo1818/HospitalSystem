using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class AdmissionStatus
    {
        [Key]
        public int AdmissionStatusId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
