using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class AllergyModel

    {

        [Key]
        public int AllergyID { get; set; }


        [Required]
        public string AllergyName { get; set; }

        [Required]
        public int ActiveingredientID { get; set; }

       




    }
}
