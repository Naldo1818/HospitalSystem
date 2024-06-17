using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DEMO.Controllers
{
    public class NurseController : Controller
    {
        public IActionResult MainPage()
        {
            return View();
        }
        public IActionResult Vitals()
        {
            return View();

        }
        public IActionResult AdmissionPageVital()
        {
            return View();
        }
        public IActionResult Discharge()
        {
            return View();
        }
        public IActionResult DischargeList()
        {
            return View();
        }
        public IActionResult ViewSurgeryBooking()
        {
            return View();
        }
        public IActionResult AdmissionDetailsWard()
        {
            return View();
        }
        public IActionResult AdmissionConditionsAllergy()
        {
            return View();
        }
        public IActionResult AdmittedPatients()
        {
            return View();
        }
        public IActionResult MedicationCollection()
        {
            return View();
        }
        public IActionResult MedicationAdministration()
        {
            return View();
        }
        public IActionResult PatientRecords()
        {
            return View();
        }
        
    }
}
