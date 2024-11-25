namespace DEMO.Models.NurseModels
{
    public class AdministeredMedicationViewModel
    {
        public int PrescriptionID { get; set; } // ID of the prescription being administered
        public int MedicationID { get; set; } // ID of the medication being administered
        public int QuantityAdministered { get; set; } // Amount of the medication administered

        // Add a property for the medication name if needed to display in the view
        public string MedicationName { get; set; }

        // Add a property for the prescribed quantity to compare with administered quantity
        public int PrescribedQuantity { get; set; }
    }
}
