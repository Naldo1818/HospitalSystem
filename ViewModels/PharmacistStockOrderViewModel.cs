﻿using System.ComponentModel.DataAnnotations;
using DEMO.Models;

namespace DEMO.ViewModels
{
    public class PharmacistStockOrderViewModel
    {
        [Key]
        public int PharmMedStockOrderID { get; set; }
        [Required]
        public int MedicationID { get; set; }



        [Required]
        public int PharmMedID { get; set; }



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

        public List<PharmacistStockOrderViewModel> PharmacistStockOrders { get; set; }




        public int qty { get; set; }

        public List<PharmMedicationStockOrder> StockOrder { get; set; }

        public List<ReceivedStockModel> ReceivedStock { get; set; }



    }
}
