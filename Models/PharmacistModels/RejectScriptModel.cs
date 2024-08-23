using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class RejectScriptModel
        
    {

        [Key]
        public int RejectionID { get; set; }
       
        [Required]
        public string RejectionReason { get; set;}
        [Required]
        public int PrescriptionID {  get; set; }

        [Required]
        public int AccountID { get; set; }


    }
}
