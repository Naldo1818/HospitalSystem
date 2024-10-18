using DEMO.Data.Migrations;
using DEMO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DEMO.ViewModels
{
    public class PharmacistViewScriptModel
    {


        //prescription

        public int PatientID {  get; set; }  

        public int PrescriptionID {  get; set; }    

        public string patientname { get; set; }

        public string patientsurname { get; set; }

        public string ActiveIngredientName { get; set; }



        public string Surgeon { get; set; }
        public string Urgency { get; set; }

        public string Take { get; set; }


        public string Status { get; set; }



        public TimeOnly Time { get; set; }

        public DateOnly DateGiven { get; set; }



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

        public DateOnly Date { get; set; } 

        public List<PharmacistViewScriptModel> combinedData { get; set; }


        public List<PharmacistViewScriptModel> Allvitals { get; set; }
        public List<PharmacistViewScriptModel> Allallergy { get; set; }
        public List<PharmacistViewScriptModel> AllCurrentMed { get; set; }
        public List<PharmacistViewScriptModel> AllConditions { get; set; }


        public List<PharmacistViewScriptModel> PrescrptionDetails {  get; set; }





































































    }
}
