using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PrescriptionMedicationViewModel
    {
        // 🔹 Patient Info
        public int PatientID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Surname { get; set; }

        // 🔹 Booking / Surgery
        public int BookingID { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public TimeOnly SurgeryTime { get; set; }

        // 🔹 Admission
        public int AdmittedPatientID { get; set; }
        public int WardID { get; set; }
        public int BedId { get; set; }

        // 🔹 Prescription
        public int PrescriptionID { get; set; }
        public string Urgency { get; set; }

        // 🔹 Medication Instructions
        [Required]
        [StringLength(500)]
        public string Instructions { get; set; }

        [Required]
        [Range(1, 100)]
        public int Quantity { get; set; }

        // 🔹 Medication
        [Required]
        [StringLength(100)]
        public string MedicationName { get; set; }

        [Required]
        [StringLength(50)]
        public string MedicationForm { get; set; }

        [Required]
        [Range(0, 6)]
        public int Schedule { get; set; }

        // 🔹 Optional / Extra Fields
        public DateOnly DateGiven { get; set; }
        public string Status { get; set; }

        public string ConditionName { get; set; }
        public int MedicationID { get; set; }
        public int ActiveingredientID { get; set; }
        public int ActiveIngredientStrength { get; set; }
        public string ActiveIngredientName { get; set; }

        // 🔹 Vitals
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

        // 🔹 Collections (keep if used)
        public List<PrescriptionMedicationViewModel> Allvitals { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedicationsInteractionsPrescription { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedicationsWithInteractions { get; set; }
        public List<PrescriptionMedicationViewModel> AllConditionMedication { get; set; }
        public List<PrescriptionMedicationViewModel> AllGoodMedications { get; set; }
        public List<PrescriptionMedicationViewModel> AllMedication { get; set; }
        public List<PrescriptionMedicationViewModel> CombinedData { get; set; }

        public PatientListViewModal PatientInfo { get; set; }

        public List<PrescriptionMedicationViewModel> Allergies { get; set; }
        public List<PrescriptionMedicationViewModel> CurrentMedications { get; set; }
        public List<PrescriptionMedicationViewModel> Conditions { get; set; }
    }
}