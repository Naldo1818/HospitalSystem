using DEMO.Models;
using DEMO.Models.PharmacistModels;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace DEMO.ViewModels
{
    public class AddMedicationViewModel
    {
        [Key]

        public int AddPharmMedID {  get; set; }

        public List<string> PharmacyMedications { get; set; } // Assuming these are strings
        public List<string> PharmMedDF { get; set; }          // Assuming these are strings
        public List<int> PharmMedSchedule { get; set; }    // Assuming these are strings


        

        public string PharmMedName { get; set; }


        public string DosageForm {  get; set; }

        public int Schedule{ get; set; }


        [Required]
        public int StockonHand { get; set; }


        [Required]
        public int Reorderlevel { get; set; }

        public int MedicationID { get; set; }


    }
}
