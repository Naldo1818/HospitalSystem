using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class DispensedScriptsModel
        
    {

        [Key]
        public int DispensedScriptsID { get; set; }
       
      
        [Required]
        public int PrescriptionID {  get; set; }

        [Required]
        public int AccountID { get; set; }


    }
}
