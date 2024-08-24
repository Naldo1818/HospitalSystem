using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Activeingredient
    {  //Create MedicationActiveIngredient Table
        [Key]
        public int ActiveingredientID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "The Active Ingredient Name can only contain letters, numbers, and spaces.")]
        public string ActiveIngredientName { get; set; }
       

    }
}
