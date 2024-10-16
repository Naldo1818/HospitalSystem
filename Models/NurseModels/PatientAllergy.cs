using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientAllergy
    {
        [Key]
        public int patientAllergyID {  get; set; }

        [Required]
        public int PatientID { get; set; }

       

        
        public int ActiveingredientID { get; set;}

    }
}
