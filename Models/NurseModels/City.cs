using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class City
    {
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }
        public int ProvinceID { get; set; }
    }
}
