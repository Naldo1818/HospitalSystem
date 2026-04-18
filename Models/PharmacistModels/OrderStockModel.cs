using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class OrderStockModel

    {

        [Key]

        public int OrderedStockID {get; set;}
        public int MedicationID { get; set; }
        public int Amount { get; set; }
        public int AccountID { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
