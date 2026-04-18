using DEMO.Models.NurseModels;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class PatientChronicMedicationVM
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string CMedicationName { get; set; }
        public string CMedicationForm { get; set; }
        public int Schedule { get; set; }
        public int patientConditionsID { get; set; }
        public int PatientID { get; set; }
        


        
       public List<ChronicMedication> AllChronicMedication { get; set; }
        public List<PatientChronicMedicationVM> PatientMedicationlist { get; set; }
    }
}
