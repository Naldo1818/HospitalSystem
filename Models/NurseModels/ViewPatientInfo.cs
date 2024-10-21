using DEMO.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class ViewPatientInfo
    {
        
        public string Name { get; set; }
        
        public string Surname { get; set; }
        public DateOnly Date { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Street { get; set; }
        public Province Province { get; set; }
        public City City { get; set; }
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
        public List<ViewPatientInfo> AllcombinedData { get; set; }
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

       
    }
}
