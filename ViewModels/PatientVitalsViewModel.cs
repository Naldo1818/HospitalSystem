using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientVitalsViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string ActiveIngredientName { get; set; }
        public string ConditionName { get; set; }
        public string MedicationName { get; set; }
        public int PatientVitalsID { get; set; }
        public int PatientID { get; set; }
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


        public List<PatientVitalsViewModel> PatientVitals { get; set; }
       
    }
}
