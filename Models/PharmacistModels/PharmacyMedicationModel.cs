using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class PharmacyMedicationModel
    {
        [Key]
        public int PharmacyMedicationlID { get; set; }

        [Required]
        public string MedicationName {  get; set; }

        [Required]
        public string DosageForm {  get; set; }

        [Required]

        public string Schedule {  get; set; }

        [Required]

        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }


        [Required]

        public string ActiveIngredientsAndStrength { get; set; }




        


    }
}
