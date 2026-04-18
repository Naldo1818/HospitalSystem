using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Org.BouncyCastle.Crypto.Utilities;

namespace DEMO.Models.NurseModels
{
    public class ContactInfo
    {
        [Key]
        public int ContactInfoID { get; set; }

        [Required]
        public int PatientID { get; set; }
        [Required]
        public int AddressId { get; set; }
        [Required]
        public string ContactNumber { get; set; }
    }
}
