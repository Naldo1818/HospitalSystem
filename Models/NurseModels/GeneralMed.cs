using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class GeneralMed
    {
        [Key]
        public int GeneralMedID { get; set; }
        [Required]
        public string GeneralMedName { get; set; }
        [ForeignKey("ActiveingredientID")]
        public int ActiveingredientID { get; set; }


    }
}
