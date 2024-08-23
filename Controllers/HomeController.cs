using DEMO.Data;
using DEMO.Data.Migrations;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("PharmacistHomePage", new { AccountID });
                    }
                   
                    else if (user.Role == "Nurse")
                    {
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("MainPage", new { AccountID });
                        
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
            var allPatients = _dbContext.PatientInfo.OrderBy(a => a.Name).ToList();
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

                return RedirectToAction("SurgeryTreatmentCodes", new { id = booking.BookingID });
            }

            // If validation fails, redisplay the form with errors
            return View("PatientList", model);
            
        }
        public IActionResult SurgeryTreatmentCodes(int id)
        {
            var combinedData = (from b in _dbContext.BookSurgery
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join stc in _dbContext.SurgeryTreatmentCode on b.BookingID equals stc.BookingID
                                join tc in _dbContext.TreatmentCodes on stc.TreatmentCodeID equals tc.TreatmentCodeID
                                where b.BookingID == id
                                select new BookingTreatmentCodesViewModel
                                {
                                    Name = p.Name,
                                    Surname = p.Surname,

                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater,

                                    TreatmentName = tc.TreatmentName,
                                    TreatmentCode = tc.TreatmentCode
                                }).ToList();

            var allTreatmentCodes = _dbContext.TreatmentCodes.OrderBy(a => a.TreatmentName).ToList();
            if (allTreatmentCodes == null)
            {
                allTreatmentCodes = new List<TreatmentCodes>();
            }

            // Create the view model
            var viewModel = new BookingTreatmentCodesViewModel
            {
                AllcombinedData = combinedData,
                AllTreatmentCodes = allTreatmentCodes,
            };
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
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddTreatmentCode(SurgeryTreatmentCode model)
        {
            if (ModelState.IsValid)
            {
                SurgeryTreatmentCode newSurgeryTreatmentCode = new SurgeryTreatmentCode
                {
                    BookingID = model.BookingID,
                    TreatmentCodeID = model.TreatmentCodeID
                };

                _dbContext.SurgeryTreatmentCode.Add(newSurgeryTreatmentCode);
                _dbContext.SaveChanges();

                // Redirect to SurgeryTreatmentCodes with the BookingID
                return RedirectToAction("SurgeryTreatmentCodes", new { id = model.BookingID });
            }

            // If validation fails, redisplay the form with errors
            return View("SurgeryTreatmentCodes", model);
        }
        

        public IActionResult CheckTreatmentCode(int bookingId)
        {
            var allTreatmentCodes = _dbContext.TreatmentCodes.OrderBy(a => a.TreatmentName).ToList();
            if (allTreatmentCodes == null)
            {
                allTreatmentCodes = new List<TreatmentCodes>();
            }
            var combinedData = (from st in _dbContext.SurgeryTreatmentCode
                                join t in _dbContext.TreatmentCodes on st.TreatmentCodeID equals t.TreatmentCodeID
                                join b in _dbContext.BookSurgery on st.BookingID equals b.BookingID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                where st.BookingID == bookingId
                                select new EditTreatmentListViewModel
                                 {
                                    TreatmentName = t.TreatmentName,
                                    TreatmentCode = t.TreatmentCode,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Name = p.Name,
                                    Surname= p.Surname
                                }).OrderBy(a => a.Name).ToList();

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var booking = _dbContext.BookSurgery.Find(bookingId);

            var viewModel = new EditTreatmentListViewModel
            {
                AllcombinedData = combinedData,
                AllTreatmentCodes = allTreatmentCodes
            };

            ViewBag.BookSurgeryID = bookingId;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult EditTreatment(SurgeryTreatmentCode model)
        {
            if (ModelState.IsValid)
            {
                SurgeryTreatmentCode newEditTreatment = new SurgeryTreatmentCode
                {
                    BookingID = model.BookingID,
                    TreatmentCodeID = model.TreatmentCodeID
                };

                _dbContext.SurgeryTreatmentCode.Add(newEditTreatment);
                _dbContext.SaveChanges();

                // Redirect to SurgeryTreatmentCodes with the BookingID
                return RedirectToAction("CheckTreatmentCode", new { id = model.BookingID });
            }

            // If validation fails, redisplay the form with errors
            return View("CheckTreatmentCode", model);
        }
        public IActionResult ListSurgery()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }

            var combinedData = (from b in _dbContext.BookSurgery
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                where b.AccountID == accountID
                                select new SurgeryListViewModel
                                {   BookingID=b.BookingID,
                                    PatientID=b.PatientID,
                                    AccountID=b.AccountID,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater
                                }).OrderBy(a => a.Name).ToList();

            var viewModel = new SurgeryListViewModel
            {
                AllcombinedData = combinedData,
               
            };

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
        public IActionResult AddPrescription(Prescription model)
        {
            if (ModelState.IsValid)
            {
                // Assuming you have a DbContext and SurgeryBooking model
                Prescription newPrescription = new Prescription
                {
                    BookingID = model.BookingID,
                    AccountID = model.AccountID,
                    DateGiven = model.DateGiven,
                    Urgency = model.Urgency,
                    Take = model.Take,
                    Status = model.Status,

                };

                _dbContext.Prescription.Add(newPrescription);
                _dbContext.SaveChanges();

                return RedirectToAction("PrescriptionMedication", new { id = newPrescription.PrescriptionID });

            }

            // If validation fails, redisplay the form with errors
            return View("ListSurgery", model);

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
        [HttpPost]
        public IActionResult EditSurgery(BookSurgery model)
        {
            if (ModelState.IsValid)
            {
                var booking = _dbContext.BookSurgery.FirstOrDefault(p => p.BookingID == model.BookingID);

                if (booking != null)
                {
                    // Update the existing account with new values
                    booking.PatientID = model.PatientID;
                    booking.AccountID = model.AccountID;
                    booking.SurgeryTime = model.SurgeryTime;
                    booking.SurgeryDate = model.SurgeryDate;
                    booking.Theater = model.Theater;

                    _dbContext.SaveChanges();
                }

                return RedirectToAction("ListSurgery");
            }

            return View(model);
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
        public IActionResult PrescriptionMedication(int id)
        {
            var combinedData = (from pr in _dbContext.Prescription
                                join bs in _dbContext.BookSurgery on pr.BookingID equals bs.BookingID
                                join p in _dbContext.PatientInfo on bs.PatientID equals p.PatientID
                                join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
                                where pr.PrescriptionID == id
                                select new PrescriptionMedicationViewModel
                                {
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Status = pr.Status,
                                    MedicationName = m.MedicationName,
                                    Quantity = mi.Quantity,
                                    MedicationForm = m.MedicationForm,
                                    Instructions = mi.Instructions
                                }).OrderBy(a => a.Name).ToList();

            var allMedication = _dbContext.Medication.OrderBy(a => a.MedicationName).ToList();
            if (allMedication == null)
            {
                allMedication = new List<Medication>();
            }
            var viewModel = new PrescriptionMedicationViewModel
            {
                CombinedData = combinedData,
                AllMedication = allMedication,
            };
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var Prescription = _dbContext.Prescription.Find(id);

            ViewBag.PrescriptionID = id;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddMedication(MedicationInstructions model)
        {
            if (ModelState.IsValid)
            {
                // IDs not Going IN
                MedicationInstructions newMedicationInstructions = new MedicationInstructions
                {
                    PrescriptionID = model.PrescriptionID,
                    MedicationID = model.MedicationID,
                    Instructions = model.Instructions,
                    Quantity = model.Quantity
             
                };

                _dbContext.MedicationInstructions.Add(newMedicationInstructions);
                _dbContext.SaveChanges();

                return RedirectToAction("PrescriptionMedication", new { id = model.PrescriptionID });
            }

            // If validation fails, redisplay the form with errors
            return View("ListSurgery", model);

        }
        public IActionResult PrescriptionList()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }

            var Prescribed = (from p in _dbContext.PatientInfo
                              join bs in _dbContext.BookSurgery
            on p.PatientID equals bs.PatientID
                              join pr in _dbContext.Prescription
                              on bs.BookingID equals pr.BookingID
                              where pr.Status == "Prescribed" && pr.AccountID == accountID
                              select new PrescriptionListViewModal
                              {
                                  IDNumber = p.IDNumber,
                                  Name = p.Name,
                                  Surname=p.Surname,
                                  DateGiven = pr.DateGiven,
                                  Urgency = pr.Urgency,
                                  Take = pr.Take,
                                  Status = pr.Status
                              }).OrderBy(a => a.Name).ToList();

            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribed = Prescribed,

            };
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
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
