using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class PharmMedModel
    {
        [Key]
        public int PharmacyMedicationlID { get; set; }

        public int MedicationID { get; set; }

        [Required]
        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }


        }
}
