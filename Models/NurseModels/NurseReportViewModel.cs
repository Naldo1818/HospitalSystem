namespace DEMO.Models.NurseModels
{
    public class NurseReportViewModel
    {
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }

        public string AdministeredMedication { get; set; }
        public int AdministeredQuantity { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }

        public List<NurseReportViewModel> AllcombinedData { get; set; }


    }
}
