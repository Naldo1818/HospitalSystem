using DEMO.Data.Migrations;
using DEMO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.ComponentModel.DataAnnotations;
using DEMO.ViewModels;
using Microsoft.Build.ObjectModelRemoting;



namespace DEMO.ViewModels
{
    public class PharmacistViewScriptModel
    {


        //prescription

        public int PatientID { get; set; }

        public int AdmittedPatientID { get; set; }

        public int PrescriptionID { get; set; }

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
        public string Condition { get; set; }

        public string patientMedication { get; set; }

        public string allergy { get; set; }


        //surgeon info
        public string SurgeonName { get; set; }

        public string SurgeonSurname { get; set; }


        //vitals
        public int Height { get; set; }

        public int Weight { get; set; }

        public int SystolicBloodPressure { get; set; }


        public int DiastolicBloodPressure { get; set; }

        public int HeartRate { get; set; }


        public double BloodOxygen { get; set; }

        public int Respiration { get; set; }

        public int BloodGlucoseLevel { get; set; }

        public double Temperature { get; set; }

        public DateOnly Date { get; set; }

        public List<PharmacistViewScriptModel> combinedData { get; set; }

        public List<PharmacistViewScriptModel> Allallergy { get; set; }

        public List<PharmacistViewScriptModel> AllCurrentMed { get; set; }

        public List<PharmacistViewScriptModel> AllConditions { get; set; }


        public string Instructions { get; set; }

        public int qty { get; set; }

        public string medication { get; set; }


        public List<string> allpresribedmeds { get; set; }



        //public List<PharmacistViewScriptModel> PrescriptionDetails {  get; set; }





































































    }
}
