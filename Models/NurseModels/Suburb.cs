using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Suburb
    {
        [Key]
        public int SuburbID { get; set; }
        
        public string Name { get; set; }
        public string PostalCode { get; set; }
    }
}
