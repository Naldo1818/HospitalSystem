﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class PharmMedicationStockOrder
    {
        [Key]
        public int MedicationReorderID { get; set; }  // Primary key (auto-increment)
        public string MedicationName { get; set; }
        public string MedicationForm { get; set; }
        public string Schedule { get; set; }
        public int StockonHand { get; set; }
        public int ReorderLevel { get; set; }
    }
}
