using System.ComponentModel.DataAnnotations;

namespace DEMO.Models
{
    public class ReceivedStockModel
    {
        [Key]
        public int ReceivedStockID { get; set; }  // Primary key (auto-increment)
        public string MedicationName { get; set; }
        public string MedicationForm { get; set; }
        public int Schedule { get; set; }
       

        public int qtyReceived { get; set; }

        public int MedicationID {  get; set; }

      
    }
}
