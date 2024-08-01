using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.PharmacistModels
{
    public class CurrentLevelModel
    {
        [Key]
        public int CurrentLevelID { get; set; }

      
        [Required]
        public int StockID { get; set; }

        [Required]
        public int LevelReceived { get; set; }


    }
}
