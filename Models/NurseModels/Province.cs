using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Province
    {
        [Key]
        public int ProvinceID { get; set; }
        public string ProvinceName { get;set; }
        [Key]
        public int CityID {  get; set; }

    }
}
