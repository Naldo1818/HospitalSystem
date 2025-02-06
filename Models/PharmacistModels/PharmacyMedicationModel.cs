using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DEMO.Models;
using DEMO.ViewModels;


namespace DEMO.Models.PharmacistModels
{
    public class PharmacyMedicationModel
    {
        [Key]
        public int PharmacyMedicationID { get; set; }
        [Required]
        public int MedicationID { get; set; }

        [Required]
        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }

        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string MedicationForm { get; set; }
        [Required]
        public int Schedule { get; set; }


    }
}
