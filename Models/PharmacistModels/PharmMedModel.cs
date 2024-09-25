﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class PharmMedModel
    {
        [Key]
        public int PharmacyMedicationlID { get; set; }

        
        public int MedicationID { get; set; }

        public int MedicationActiveingredientID { get; set; }

        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }


        }
}
