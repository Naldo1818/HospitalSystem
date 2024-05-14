using DEMO.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DEMO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Page1()
        {
            return View();
        }
        public IActionResult SurgeonHome()
        {
            return View();
        }
        public IActionResult PatientList()
        {
            return View();
        }
        
        public IActionResult patientAddmision()
        {
            return View();
        }
        public IActionResult PatientAdd()
        {
            return View();
        }
        public IActionResult BookSurgery()
        {
            return View();
        }
        public IActionResult SurgeryTreatmentCode()
        {
            return View();
        }
        public IActionResult CheckTreatmentCode()
        {
            return View();
        }
        public IActionResult ListSurgery()
        {
            return View();
        }
        public IActionResult vitalsAndHistory()
        {
            return View();
        }
        public IActionResult EditSurgery()
        {
            return View();
        }
        public IActionResult Prescription()
        {
            return View();
        }
        public IActionResult PrescriptionMedication()
        {
            return View();
        }
        public IActionResult PrescriptionList()
        {
            return View();
        }
        
        public IActionResult MedicationInteraction()
        {
            return View();
        }

        public IActionResult ChangeMedication()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
