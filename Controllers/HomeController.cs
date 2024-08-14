using DEMO.Data;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DEMO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HomeController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Index(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Accounts.SingleOrDefault(u => u.Username == login.Username && u.Password == login.Password);

                if (user != null)
                {
                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("AdminLanding");
                    }
                    else if (user.Role == "Surgeon")
                    {
                       
                        return RedirectToAction("SurgeonHome");
                    }
                    else if (user.Role == "Pharmacist")
                    {
                        
                        return RedirectToAction("PharmacistHome");
                    }
                   
                    else if (user.Role == "Nurse")
                    {
                        return RedirectToAction("NurseHome");
                    }
                   
                    //else if (user.Role == "Patient")
                    //{
                    //int idAccount = _dbContext.PatientsInfo.FirstOrDefault(p => p.Email == model.Email)?.IDPatient ?? 0;
                    //    return RedirectToAction("PatientLanding", new { idPatient });
                    //}
                }
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(login);
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
        public IActionResult InfoSurgeon()
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
