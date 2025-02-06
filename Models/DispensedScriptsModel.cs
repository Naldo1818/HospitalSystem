using System.ComponentModel.DataAnnotations;


namespace DEMO.Models
{
    public class DispensedScriptsModel
        
    {

        [Key]
        public int DispensedScriptsID { get; set; }
       
        
        public int PrescriptionID {  get; set; }

       
        public int AccountID { get; set; }


    }
}
