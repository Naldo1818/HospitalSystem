using DEMO.Models;
using DEMO.Models.NurseModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class BookedPatientInfo
    {
        [Key]
        public int BookedPatientInfoID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }
        public DateOnly Date { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Street { get; set; }
        public Province Province { get; set; }
        public City City  { get; set; }
        public Suburb Suburb { get; set; }
        public PatientVitals Vitals { get; set; }
        public Ward Ward { get; set; }
        public Bed Bed { get; set; }
        public List<Medication> Medications { get; set; }
        public List<Activeingredient> Allergies { get; set; }
        public List<Condition> Conditions { get; set; }
        public int BookingID { get; set; }



        public int PatientID { get; set; }
    
        public int AdmittedPatientID { get; set; }
        public int AccountID { get; set; }
        
        public string SurgeryTime { get; set; }
        public DateOnly SurgeryDate { get; set; }
        public TimeOnly Time { get; set; }
        public string SurgeonName { get; set; }
        public string SurgeonSurname { get; set; }
        public string Gender { get; set; }
        public string Theater { get; set; }
        public string WardName { get; set; }
        public string AdmissionStatusDescription { get; set; }
        public int BedNumber { get; set; }
        public List<BookedPatientInfo> AllcombinedData { get; set; }
        public string ActiveIngredientName { get; set; }
        public string ConditionName { get; set; }
        public string MedicationName { get; set; }
        public int SystolicBloodPressure { get; set; }
        public int DiastolicBloodPressure { get; set; }
        public int HeartRate { get; set; }
        public double BloodOxygen { get; set; }
        public int Respiration { get; set; }
        public int BloodGlucoseLevel { get; set; }
        public double Temperature { get; set; }

        public List<BookedPatientInfo> Allvitals { get; set; }
        public List<BookedPatientInfo> Allallergy { get; set; }
        public List<BookedPatientInfo> AllCurrentMed { get; set; }
        public List<BookedPatientInfo> AllConditions { get; set; }


    }
}
