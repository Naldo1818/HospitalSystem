namespace DEMO.Models.NurseModels
{
    public class AdministerMedication
    {
        public int AdministerMedicationID { get; set; }
        public int AdmittedPatientID {  get; set; }
        public int PrescriptionID { get; set; }
        public int AccountID { get; set; }
        public int MedicationID {  get; set; }
        public int AdministerQuantity { get; set; }
        public TimeOnly Time { get; set; }
        public DateOnly Date { get; set; }


    }
}
