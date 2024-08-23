using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Status
    {
        [Key]
        public int StatusID { get; set; }
        [Required]  
        public string Description { get; set; }
    }
}
