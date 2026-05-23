using System;
using System.Collections.Generic;

namespace DEMO.ViewModels
{
    public class PrescriptionAdministerViewModel
    {
        public int PrescriptionID { get; set; }
        public int AdmittedPatientID { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public DateTime? DateGiven { get; set; }
        public string Status { get; set; }
        public string Urgency { get; set; }
        public string PharmacistName { get; set; }
        public string PharmacistSurname { get; set; }
        public string Notes { get; set; }
        public List<MedicationDetail> Medications { get; set; }
    }

    public class MedicationDetail
    {
        public int MedicationID { get; set; }
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public int Schedule { get; set; }
        public string MedicationForm { get; set; }
        public string Instructions { get; set; }
        public bool IsAdministered { get; set; }
        public int? AdministeredQuantity { get; set; }
        public DateTime? AdministeredDate { get; set; }
        public string AdministeredByName { get; set; }
    }
}