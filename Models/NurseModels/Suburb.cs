using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Suburb
    {
        [Key]
        public int SuburbID { get; set; }
        
        public string Name { get; set; }
        public int PostalCode { get; set; }
        public int CityID {  get; set; }
    }
}
