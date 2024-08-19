using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class Activeingredient
    {  //Create MedicationActiveIngredient Table
        [Key]
        public int ActiveingredientID { get; set; }

        [Required]
        public string ActiveIngredientName { get; set; }
       

    }
}
