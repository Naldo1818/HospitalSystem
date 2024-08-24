using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Status
    {
        [Key]
        public int StatusID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Description can only contain letters and spaces.")]
        public string Description { get; set; }
    }
}
