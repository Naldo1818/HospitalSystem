using DEMO.Data.Migrations;
using DEMO.Models.NurseModels;

namespace DEMO.ViewModels
{
    public class PharmacistViewScriptModel
    {


        //prescription

        public string patientname { get; set; }

        public string patientsurname { get; set; }

        public DateOnly DateGiven { get; set; }


        public string Surgeon { get; set; }
        public string Urgency { get; set; }

        public string Take { get; set; }


        public string Status { get; set; }




        //medical history
        public string Condition  { get; set; }

        public string  patientMedication { get; set; }

        public string allergy { get; set; }



      //vitals
        public int Height { get; set; }
       
        public int Weight { get; set; }
      
        public int SystolicBloodPressure { get; set; }
      

        public int DiastolicBloodPressure { get; set; }
        
        public int HeartRate { get; set; }

      
        public int BloodOxygen { get; set; }
       
        public int Respiration { get; set; }
        
        public int BloodGlucoseLevel { get; set; }
        
        public int Temperature { get; set; }











    }
}
