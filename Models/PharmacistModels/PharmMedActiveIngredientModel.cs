using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class PharmMedActiveIngredientModel
    {
        [Key]
        public int PharmMedActiveIngredient { get; set; }


       

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Medication Name can only contain letters, numbers, and spaces.")]
        public string Name { get; set; }

        [Required]
        public int Strength { get; set; }

       public int PharmacyMedicationID { get; set; }

    }
}
