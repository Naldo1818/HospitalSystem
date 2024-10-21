namespace DEMO.Models.NurseModels
{
    public class ViewVitals
    {
        public int PatientVitalsID { get; set; }

        public int AdmittedPatientID { get; set; }
        public int PatientID { get; set; }

   
        public int SystolicBloodPressure { get; set; }
     

        public int DiastolicBloodPressure { get; set; }
        
        public int HeartRate { get; set; }

      
        public double BloodOxygen { get; set; }
       
        public int Respiration { get; set; }
    
        public int BloodGlucoseLevel { get; set; }
        
        public double Temperature { get; set; }
       
        public TimeOnly time { get; set; }
    }
}
