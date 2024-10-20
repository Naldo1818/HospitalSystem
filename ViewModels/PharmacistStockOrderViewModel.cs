using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PharmacistStockOrderViewModel
    {
        [Key]
        public int PharmMedStockID { get; set; }
        [Required]
        public int MedicationID { get; set; }

        

        public string DosageForm { get; set; }
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


        public List<int> Schedules { get; set; }


        public List<string> DosageForms { get; set; }

        public List<PharmacistStockOrderViewModel> PharmacistStockOrders { get;set; }

                 
                        

        //public List<PharmacyMedicationViewModel> combinedinfo { get; set; }

    }
}
