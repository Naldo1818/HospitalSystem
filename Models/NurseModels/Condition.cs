using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class Condition
    {
        [Key]
        public int ConditionID { get; set; }

        [Required]
        public string ConditionName { get; set; }

        [ForeignKey("ActiveingredientID")]
        public int ActiveingredientID { get; set; }
    }
}
