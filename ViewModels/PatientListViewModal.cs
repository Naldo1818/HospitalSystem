using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientListViewModal
    {
        //RegisterPatient
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [StringLength(13, MinimumLength = 13)]
        public string IDNumber { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string ContactNumber { get; set; }

        //BookSurgery
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




        //List
        public PatientInfo patientInfo { get; set; }
        public List<PatientInfo> AllPatients { get; set; }
    }
}
