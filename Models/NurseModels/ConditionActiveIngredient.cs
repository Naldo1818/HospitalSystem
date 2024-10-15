using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class ConditionActiveIngredient
    {
        [Key]
        public int ConditionActiveIngredientID { get; set; }

        [Required]
        public int ConditionID { get; set; }
        [Required]
        public int ActiveingredientID { get; set; }


    }
}
