using DEMO.Models;
using DEMO.ViewModels;
using System.ComponentModel.DataAnnotations;


namespace DEMO.ViewModels
{
    public class ViewActivePrescriptionsModel
    {
        [Required]
        public int PrescriptionID { get; set; }

        [Required(ErrorMessage = "ID Number is required.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "ID Number must be exactly 13 characters.")]
        [RegularExpression(@"^\d{13}$", ErrorMessage = "ID Number must be exactly 13 digits.")]
        public string IDNumber { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(100, ErrorMessage = "Surname cannot be longer than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Surname can only contain letters and spaces.")]
        public string Surname { get; set; }

        public string SurgeonName {  get; set; }

        public string SurgeonSurname { get; set; }
        public string Urgency { get; set; }
        public string Status { get; set; }
        public string Take { get; set; }
        public DateOnly DateGiven { get; set; }
        public List<ViewActivePrescriptionsModel> combinedData { get; set; }

    }
}
