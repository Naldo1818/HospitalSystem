using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class City
    {
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }
        [Key]
        public int Suburb { get; set; }
    }
}
