using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Bed
    {
        [Key]
        public int BedId { get; set; }
        public int Number { get; set; }
        public bool Active { get; set; }
        public int WardID { get; set; }

    }
}
