using DEMO.Data;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
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
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("SurgeonHome", new { AccountID });
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
        public IActionResult SurgeonHome(int accountId)
        {
            // Try to get data from session first
            var name = HttpContext.Session.GetString("UserName");
            var surname = HttpContext.Session.GetString("UserSurname");
            var email = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(email))
            {
                // Use existing session data
                ViewBag.UserName = name;
                ViewBag.UserSurname = surname;
                ViewBag.UserEmail = email;
            }
            else
            {
                // Retrieve from database if not in session
                var surgeon = _dbContext.Accounts
                    .Where(a => a.AccountID == accountId)
                    .Select(a => new SurgeonViewModel
                    {
                        Name = a.Name,
                        Surname = a.Surname,
                        Email = a.Email
                    })
                    .SingleOrDefault();

                if (surgeon == null)
                {
                    return NotFound();
                }

                // Store user data in session
                HttpContext.Session.SetString("UserName", surgeon.Name);
                HttpContext.Session.SetString("UserSurname", surgeon.Surname);
                HttpContext.Session.SetString("UserEmail", surgeon.Email);

                ViewBag.UserName = surgeon.Name;
                ViewBag.UserSurname = surgeon.Surname;
                ViewBag.UserEmail = surgeon.Email;
            }

            return View();
        }


        public IActionResult PatientList()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        
        public IActionResult patientAddmision()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PatientAdd()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult BookSurgery()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult SurgeryTreatmentCode()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult CheckTreatmentCode()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult ListSurgery()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult vitalsAndHistory()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult EditSurgery()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult Prescription()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PrescriptionMedication()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PrescriptionList()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        
        public IActionResult MedicationInteraction()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult InfoSurgeon()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }

        public IActionResult ChangeMedication()
        {
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
