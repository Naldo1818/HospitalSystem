using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class SurgeonViewModel
    {
        [Required]
      
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
