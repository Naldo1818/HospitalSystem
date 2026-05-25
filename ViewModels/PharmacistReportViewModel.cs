using System;
using System.Collections.Generic;

namespace DEMO.ViewModels
{
    public class PharmacistReportViewModel
    {
        public List<PharmacistReportItem> DispensedMedications { get; set; }
        public List<PharmacistReportItem> RejectedMedications { get; set; }

        // Combined property for the view
        public List<PharmacistReportItem> AllcombinedData { get; set; }
    }

    public class PharmacistReportItem
    {
        public DateTime PrescriptionDate { get; set; }

        // Properties for the view
        public DateTime Date { get; set; }  // For the view to display
        public string Patient { get; set; }
        public string Surgeon { get; set; }
        public string Medication { get; set; }
        public string MedicationName { get; set; }  // For the view
        public int qty { get; set; }
        public int Quantity { get; set; }  // For the view
        public string status { get; set; }
        public string RejectionReason { get; set; }
        public string Urgency { get; set; }
    }
}