using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Ward
    {
        [Key]
        public int WardId { get; set; }
        [Required]
        public string WardName { get; set; }

        [Required]
        public int BedID { get; set; }

    }
}
