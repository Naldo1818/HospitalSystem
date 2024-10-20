namespace DEMO.ViewModels
{
    public class PharmacistReportViewModel
    {
        public DateOnly PrescriptionDate { get; set; }
        public string Patient { get; set; }

        public string Surgeon { get; set; }

        public string Medication { get; set; }


        public int qty { get; set; }

        public string status { get; set; }

        public string specificmedication { get; set; }

        public int amountdispensed { get; set; }    

        


        public List<PharmacistReportViewModel> AllcombinedData { get; set; }
    }
}
