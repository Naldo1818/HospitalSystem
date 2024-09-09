using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }
        public int ProvinceID { get; set; }
        public int CityID { get; set; }
        public int SuburbID { get; set; }
    }
}
