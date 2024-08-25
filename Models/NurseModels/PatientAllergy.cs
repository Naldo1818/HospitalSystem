using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientAllergy
    {
        [Key]
        public int patientAllergyID {  get; set; }

        [Required]
        public int AdmittedPatientID { get; set; }

        [Required]
        public string ActiveingredientID { get; set;}

    }
}
