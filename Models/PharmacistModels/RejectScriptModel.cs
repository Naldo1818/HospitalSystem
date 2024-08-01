using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class RejectScriptModel
        
    {

        [Key]
        public int RejectionID { get; set; }


        [Key]
        public int PrescriptionID { get; set; }

        [Required]
        public string RejectionReason { get; set;}
        
        
    }
}
