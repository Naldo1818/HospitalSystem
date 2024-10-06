using DEMO.Models;
using DEMO.Models.PharmacistModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class AddMedicationViewModel
    {
        [Key]

        public int AddPharmMedID {  get; set; }

        public List<Medication> PharmacyMedications { get; set; }   


        public List<Medication> PharmMedDF {  get; set; }


        public List<Medication> pharmMedSchedule { get; set; }

        public int StockonHand { get; set; }


        public int Reorderlevel { get; set; }

        public int MedicationID { get; set; }


    }
}
