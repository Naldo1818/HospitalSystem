using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class RejectedScriptsModel
        
    {

        [Key]
        public int RejectionID { get; set; }
       
        [Required]
        public string RejectionReason { get; set;}
        
        public int PrescriptionID {  get; set; }

       
        public int AccountID { get; set; }


    }
}
