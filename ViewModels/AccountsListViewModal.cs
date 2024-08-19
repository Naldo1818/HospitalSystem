using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class AccountsListViewModal
    {//RegisterPatient
        [Required]
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
        public int RegistrationNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string status { get; set; }
        [Required]
        public string Role { get; set; }



        //List
        public Accounts accounts { get; set; }
        public List<Accounts> AllAccounts { get; set; }
    }
}

