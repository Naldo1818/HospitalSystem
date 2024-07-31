using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Accounts
    {
        [Key]
        public int AccountID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int ContactNumber { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }





    }
}
