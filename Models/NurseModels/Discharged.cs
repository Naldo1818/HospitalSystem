using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class Discharged
    {
        [Key]
        public int DischargeId { get; set; }
        public int BookingID { get; set; }
        public bool Note { get; set; }
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }
    }
}
