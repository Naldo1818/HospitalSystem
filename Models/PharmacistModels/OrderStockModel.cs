using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class OrderStockModel

    {

        [Key]
        public int StockOrderID { get; set; }

        public string medName {  get; set; }

        public string dosageform {  get; set; }

        public int stockonhand {  get; set; }

        public int amounttoorder {  get; set; }
        

        public int PharmacyMedicationlID { get; set; }






        [Required]
        public string Status { get; set; }

    }
}
