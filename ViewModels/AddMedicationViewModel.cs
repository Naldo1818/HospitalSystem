using DEMO.Models;
using DEMO.Models.PharmacistModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class AddMedicationViewModel
    {
        [Key]

        public int AddPharmMedID {  get; set; }

        public List<string> PharmacyMedications { get; set; } // Assuming these are strings
        public List<string> PharmMedDF { get; set; }          // Assuming these are strings
        public List<int> PharmMedSchedule { get; set; }    // Assuming these are strings

        public int StockonHand { get; set; }


        public int Reorderlevel { get; set; }

        public int MedicationID { get; set; }


    }
}
