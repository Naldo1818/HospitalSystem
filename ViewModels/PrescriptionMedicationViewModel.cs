﻿using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PrescriptionMedicationViewModel
    {
        public int Medid { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Instructions cannot be longer than 500 characters.")]
        public string Instructions { get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; }
        public DateOnly DateGiven { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Status cannot be longer than 50 characters.")]
        public string Status { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Schedule must be between 0 and 6.")]
        public int Schedule { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Medication Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Medication Name can only contain letters and spaces.")]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Medication Form cannot be longer than 50 characters.")]
        public string MedicationForm { get; set; }
        public int MedicationID { get; set; }
        public int ActiveingredientID { get; set; }
        public int ActiveIngredientStrength { get; set; }
        public string ActiveIngredientName { get; set; }

        public int Height { get; set; }
        public int Weight { get; set; }
        public int SystolicBloodPressure { get; set; }
        public int DiastolicBloodPressure { get; set; }
        public int HeartRate { get; set; }
        public double BloodOxygen { get; set; }
        public int Respiration { get; set; }
        public int BloodGlucoseLevel { get; set; }
        public double Temperature { get; set; }
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }



        public int PrescriptionID { get; set; }
        public List<PrescriptionMedicationViewModel> Allvitals { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedicationsInteractionsPrescription { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedicationsWithInteractions { get; set; }
        public List<PrescriptionMedicationViewModel> AllConditionMedication { get; set; }
        public List<PrescriptionMedicationViewModel> AllGoodMedications { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedication { get; set; }
        public List<PrescriptionMedicationViewModel> CombinedData { get; set; }
        public PatientListViewModal PatientInfo { get; set; }
        public List<PatientAllergyViewModel> Allergies { get; set; }
        public List<PatientAllergyViewModel> CurrentMedications { get; set; }
        public List<PatientAllergyViewModel> Conditions { get; set; }
    }
}
