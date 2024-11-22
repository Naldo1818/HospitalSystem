﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PharmacyMedicationViewModel
    {
        [Key]
        public int PharmacyMedicationID { get; set; }


        [Required]
        public int MedicationID { get; set; }

        [Required(ErrorMessage = "Stock on Hand amount is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock on hand amount must be a numerical value and not less than 0")]
        public int StockonHand { get; set; }



        [Required(ErrorMessage = "Reorder Level is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Reorder Level must be greater than 0.")]
        public int ReorderLevel { get; set; }





        [Required(ErrorMessage = "Medication Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Medication Name can only contain letters and spaces.")]
        public string MedicationName { get; set; }


        //public string activeingredient { get; set; }


        public string MedicationForm { get; set; }

        [Required]
        public int Schedule { get; set; }


        public List<int> Schedules { get; set; }    


        public List<string> DosageForms { get; set; }

        public List<string> ActiveIngredientsDropdown { get; set; }

        public List<PharmacyMedicationViewModel> combinedinfo { get; set; }


  
    }
}
