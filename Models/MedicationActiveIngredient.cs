using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class MedicationActiveIngredient
    {
        [Key]
        public int MedicationActiveingredientID { get; set; }
        [Required]
        public int MedicationID { get; set; }
        [Required]
        public int ActiveingredientID { get; set; }
        [Required]
        public int ActiveIngredientStrength { get; set; }
       
    }
}
