using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Activeingredient
    {
        [Key]
        public int ActiveingredientID { get; set; }

        [Required]
        public int MedicationID { get; set; }
        [Required]
        public string ActiveIngredientName { get; set; }
        [Required]
        public int ActiveIngredientStrength { get; set; }



    }
}
