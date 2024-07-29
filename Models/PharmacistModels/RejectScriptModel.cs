using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class RejectScriptModel
        
    {

        [Key]
        public int PatientID { get; set; }


        [Required]
        public string PatientName { get; set; }

        [Required]
        public string PatientSurname { get; set;}

        [Required]
        public string RejectionReason { get; set;}
        
        
    }
}
