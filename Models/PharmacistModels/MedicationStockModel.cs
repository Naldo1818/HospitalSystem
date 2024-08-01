using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class MedicationStockModel

    {

        [Key]
        public int StockID { get; set; }


        [Required]
        public int MedicationID { get; set; }

        [Required]
        public string Schedule { get; set;}

        [Required]
        public int CurrentLevel { get; set;}

        [Required]
        public int ReorderLevel { get; set;}


        
        
    }
}
