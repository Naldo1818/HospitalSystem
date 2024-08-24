using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class EmailVital
    {
        public int AccountID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        
        
        public int PatientID { get; set; }

        [Required]
        public List<RetakeVitals> Vitals { get; set; }
        public string Notes { get; set; }

    }
}
