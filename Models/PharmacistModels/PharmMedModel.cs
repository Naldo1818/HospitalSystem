﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class PharmMedModel
    {
        [Key]
        public int PharmacyMedicationlID { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Medication Name can only contain letters, numbers, and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Dosage form can only contain letters, numbers, and spaces.")]
        public string DosageForm { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Schedule must be between 0 and 6.")]
        public int Schedule { get; set; }




        public int MedicationID { get; set; }

        [Required]
        public int StockonHand { get; set; }

        [Required]
        public int ReorderLevel { get; set; }


        }
}
