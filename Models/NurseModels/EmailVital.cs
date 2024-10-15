using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class EmailVital
    {
        public int AccountID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string FullName { get; set; }
        public PatientInfo PatientFullName { get; set; }

        public int BookingID { get; set; }
        public string SurgeonEmail { get; set; }

        public string SurgeonRole { get; set; }
        public int PatientID { get; set; }

        public int Height { get; set; }
        public int Weight { get; set; }
        public int SystolicBloodPressure { get; set; }
        public int DiastolicBloodPressure { get; set; }
        public int HeartRate { get; set; }
        public int BloodOxygen { get; set; }
        public int Respiration { get; set; }
        public int BloodGlucoseLevel { get; set; }
        public int Temperature { get; set; }
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }

        public int Bed { get; set; }
        public string WardName { get; set; }
        
        public string Notes { get; set; }
        


    }
}
