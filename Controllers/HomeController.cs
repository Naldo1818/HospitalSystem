using DEMO.Data;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
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
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("AdminHome", "Admin", new { AccountID });

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
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var name = HttpContext.Session.GetString("UserName");
            var surname = HttpContext.Session.GetString("UserSurname");
            var email = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(accountID) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(email))
            {
                // Use existing session data
                ViewBag.UserName = accountID;
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
                    {   AccountID = a.AccountID,
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
                HttpContext.Session.SetString("UserAccountId", surgeon.AccountID.ToString());
                HttpContext.Session.SetString("UserName", surgeon.Name);
                HttpContext.Session.SetString("UserSurname", surgeon.Surname);
                HttpContext.Session.SetString("UserEmail", surgeon.Email);

                ViewBag.UserName = surgeon.AccountID.ToString();
                ViewBag.UserName = surgeon.Name;
                ViewBag.UserSurname = surgeon.Surname;
                ViewBag.UserEmail = surgeon.Email;
            }

            return View();
        }


        public IActionResult PatientList()
        {
            var allPatients = _dbContext.PatientInfo.ToList();
            var viewModel = new PatientListViewModal
            {
                AllPatients = allPatients,

            };

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult RegisterPatient(PatientInfo model)
        {
            if (ModelState.IsValid)
            {
                PatientInfo newPatient = new PatientInfo
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    IDNumber = model.IDNumber,
                    Gender = model.Gender,
                    Email = model.Email,
                    ContactNumber = model.ContactNumber
                };

                _dbContext.PatientInfo.Add(newPatient);
                _dbContext.SaveChanges();

                return RedirectToAction("PatientList");
            }

            // If validation fails, redisplay the form with errors
            return View("PatientList", model);
        }
    
        public IActionResult patientAddmision()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PatientAdd()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        [HttpPost]
        public IActionResult BookSurgery(BookSurgery model)
        {
            if (ModelState.IsValid)
            {
                // Assuming you have a DbContext and SurgeryBooking model
                BookSurgery booking = new BookSurgery
                {
                    PatientID = model.PatientID,
                    AccountID = model.AccountID,
                    SurgeryTime = model.SurgeryTime,
                    SurgeryDate = model.SurgeryDate,
                    Theater = model.Theater,
                  
                };

                _dbContext.BookSurgery.Add(booking);
                _dbContext.SaveChanges();

                return RedirectToAction("SurgeryTreatmentCode", new { id = booking.BookingID });
            }

            // If validation fails, redisplay the form with errors
            return View("PatientList", model);
            
        }
        public IActionResult SurgeryTreatmentCode(int id)
        {
            var booking = _dbContext.BookSurgery.Find(id);
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.BookSurgeryID = id;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult CheckTreatmentCode()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult ListSurgery()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult vitalsAndHistory()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult EditSurgery()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult Prescription()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PrescriptionMedication()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PrescriptionList()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        
        public IActionResult MedicationInteraction()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult InfoSurgeon()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }

        public IActionResult ChangeMedication()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
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
