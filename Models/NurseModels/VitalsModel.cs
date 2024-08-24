using Org.BouncyCastle.Bcpg.Sig;
using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class VitalsModel
    {
        [Key]
        public int VitalsID { get; set; }
        [Required]
        
        public string VitalsName { get; set; }
    }
}
