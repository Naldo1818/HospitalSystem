using DEMO.Models;
using System.ComponentModel.DataAnnotations;

namespace DEMO.ViewModels
{
    public class BookedPatientInfo
    {
        [Key]
        public int BookedPatientInfoID { get; set; }
       public string Name { get; set; }
       public string Surname { get; set; }
        public DateOnly Date { get; set; }
    }
}
