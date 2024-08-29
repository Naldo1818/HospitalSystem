using DEMO.Models;
using DEMO.Models.NurseModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class BookedPatientInfo
    {
        [Key]
        public int BookedPatientInfoID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly Date { get; set; }
        public string Street { get; set; }
        public Province Province { get; set; }
        public City City  { get; set; }
        public Suburb Suburb { get; set; }
        public PatientVitals Vitals { get; set; }
        public Ward Ward { get; set; }
        public Bed Bed { get; set; }
        public List<Medication> medications { get; set; }
        public List<PatientAllergy> patientAllergies { get; set; }
        public List<Condition> conditions { get; set; }


    }
}
