using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class ActivePrescriptionsModel

    {

        [Key]
        public int PatientID { get; set; }


        [Required]
        public string PatientName { get; set; }

        [Required]
        public string PatientSurname { get; set;}

        [Required]
        public int Ward { get; set;}

        [Required]
        public string Nurse { get; set;}

        [Required]
        public string Urgent { get; set;}   
        
        
    }
}
