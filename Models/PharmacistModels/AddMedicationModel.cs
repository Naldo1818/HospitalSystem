﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class AddMedicationModel

    {

        [Key]
        public int ID { get; set; }


        [Required]
        public string MedicationName { get; set; }

        [Required]
        public string DosageForm { get; set;}

        [Required]
        public string Schedule { get; set;}

        [Required]
        public int CurrentLevel { get; set;}

        [Required]
        public int ReorderLevel { get; set;}

        [Required]
        public string ActiveIngredients { get; set;}


        
        
    }
}
