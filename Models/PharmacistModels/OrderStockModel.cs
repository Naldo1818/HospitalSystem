using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class OrderStockModel

    {

        [Key]
        public int StockOrderID { get; set; }

  
        

        public int PharmacyMedicationlID { get; set; }


        [Required]
        public string Status { get; set; }

    }
}
