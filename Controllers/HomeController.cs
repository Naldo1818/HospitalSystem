using DEMO.Data;
using DEMO.Data.Migrations;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
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


                        var surgeon = _dbContext.Accounts
               .Where(a => a.AccountID == AccountID)
               .Select(a => new SurgeonViewModel
               {
                   AccountID = a.AccountID,
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

                        ViewBag.AccountId = surgeon.AccountID;


                        ViewBag.UserName = surgeon.Name;
                        ViewBag.UserSurname = surgeon.Surname;
                        ViewBag.UserEmail = surgeon.Email;


                        return RedirectToAction("SurgeonHome");
                    }
                    else if (user.Role == "Pharmacist")
                    {
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("PharmacistHomePage","Pharmacist", new { AccountID });
                    }
                   
                    else if (user.Role == "Nurse")
                    {
                        int AccountID = _dbContext.Accounts.FirstOrDefault(p => p.Username == login.Username)?.AccountID ?? 0;
                        return RedirectToAction("MainPage","Nurse", new { AccountID });
                        
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
        public IActionResult SurgeonHome()
        {
           
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                 select pr).Count();
            
            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
      
        }
        public IActionResult PatientList(string idNumber)
        {
            // Fetch all patients
            var allPatients = _dbContext.PatientInfo.OrderBy(a => a.Name).ToList();

            // Filter patients by ID number if provided
            if (!string.IsNullOrEmpty(idNumber))
            {
                allPatients = allPatients.Where(p => p.IDNumber.Contains(idNumber)).ToList();
            }

            var viewModel = new PatientListViewModal
            {
                AllPatients = allPatients
            };

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
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
        public IActionResult PatientAdd()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
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
                var patient = _dbContext.PatientInfo.FirstOrDefault(p => p.PatientID == booking.PatientID);

                if (patient != null)
                {
                    // Redirect with BookingID, PatientID, Name, and Surname
                    return RedirectToAction("SurgeryTreatmentCodes", new
                    {
                        bookingId = booking.BookingID,
                        patientID = patient.PatientID,
                        name = patient.Name,
                        surname = patient.Surname
                    });
                }
            }

            // If validation fails, redisplay the form with errors
            return View("PatientList", model);
            
        }
        public IActionResult SurgeryTreatmentCodes(int bookingId)
        {
            // Get all treatment codes, ordered by name
            var allTreatmentCodes = _dbContext.TreatmentCodes.OrderBy(a => a.TreatmentName).ToList();

            // Query to fetch the combined data
            var combinedData = (from st in _dbContext.SurgeryTreatmentCode
                                join t in _dbContext.TreatmentCodes on st.TreatmentCodeID equals t.TreatmentCodeID
                                join b in _dbContext.BookSurgery on st.BookingID equals b.BookingID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                where st.BookingID == bookingId
                                select new BookingTreatmentCodesViewModel
                                {
                                    BookingID = b.BookingID,
                                    btcID = st.BookingTreatmentCodeID,
                                    TreatmentName = t.TreatmentName,
                                    TreatmentCode = t.TreatmentCode,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Name = p.Name,
                                    Surname = p.Surname
                                }).OrderBy(a => a.Name).ToList();

            // Retrieve user information from session
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Prepare the view model
            var viewModel = new BookingTreatmentCodesViewModel
            {
                AllcombinedData = combinedData,
                AllTreatmentCodes = allTreatmentCodes,
                BookingID = bookingId
            };
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            // Set ViewBag properties
            ViewBag.BookSurgeryID = bookingId;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddTreatmentCode(int bookingID, int treatmentCodeID)
        {
            // Create a new SurgeryTreatmentCode entity
            var newSurgeryTreatmentCode = new SurgeryTreatmentCode
            {
                BookingID = bookingID,
                TreatmentCodeID = treatmentCodeID
            };

            // Add the new treatment code to the database
            _dbContext.SurgeryTreatmentCode.Add(newSurgeryTreatmentCode);
            _dbContext.SaveChanges();

            // Redirect back to the SurgeryTreatmentCodes view with the booking ID
            return RedirectToAction("SurgeryTreatmentCodes", new { bookingId = bookingID });
        }
        [HttpPost]
        public IActionResult RemoveTreatmentCode(int btcID, int bookingID)
        {

            // Find the surgery treatment code to delete
            var surgeryTreatmentCode = _dbContext.SurgeryTreatmentCode
                .FirstOrDefault(st => st.BookingTreatmentCodeID == btcID);

            if (surgeryTreatmentCode == null)
            {
                // Handle the case where the treatment code is not found
                return NotFound();
            }

            // Retrieve the associated booking and patient information
            var booking = _dbContext.BookSurgery.FirstOrDefault(b => b.BookingID == bookingID);
            var patient = _dbContext.PatientInfo.FirstOrDefault(p => p.PatientID == booking.PatientID);

            if (patient == null)
            {
                return NotFound("Patient information not found.");
            }

            // Remove the treatment code from the database
            _dbContext.SurgeryTreatmentCode.Remove(surgeryTreatmentCode);
            _dbContext.SaveChanges();

            // Redirect back to the CheckTreatmentCode view with the patient name and surname
            return RedirectToAction("SurgeryTreatmentCodes", new { bookingId = bookingID, name = patient.Name, surname = patient.Surname });
        }

        public IActionResult CheckTreatmentCode(int bookingId, string name, string surname)
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
                                    BookingID = b.BookingID,
                                    btcID = st.BookingTreatmentCodeID,
                                    TreatmentName = t.TreatmentName,
                                    TreatmentCode = t.TreatmentCode,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Name = p.Name,
                                    Surname = p.Surname
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
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();
            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.BookSurgeryID = bookingId;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.PatientName = name;
            ViewBag.PatientSurname = surname;

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult EditTreatment(int bookingID, int treatmentCodeID)
        {
            // Create a new SurgeryTreatmentCode entity
            var newSurgeryTreatmentCode = new SurgeryTreatmentCode
            {
                BookingID = bookingID,
                TreatmentCodeID = treatmentCodeID
            };

            // Add the new treatment code to the database
            _dbContext.SurgeryTreatmentCode.Add(newSurgeryTreatmentCode);
            _dbContext.SaveChanges();

            // Retrieve the associated booking and patient information
            var booking = _dbContext.BookSurgery.FirstOrDefault(b => b.BookingID == bookingID);
          

            // Redirect back to the CheckTreatmentCode view with the patient name and surname
            return RedirectToAction("CheckTreatmentCode", new { bookingId = bookingID });
        }

        [HttpPost]
        public IActionResult DeleteTreatmentCode(int btcID, int bookingID)
        {
            // Find the surgery treatment code to delete
            var surgeryTreatmentCode = _dbContext.SurgeryTreatmentCode
                .FirstOrDefault(st => st.BookingTreatmentCodeID == btcID);

            if (surgeryTreatmentCode == null)
            {
                // Handle the case where the treatment code is not found
                return NotFound();
            }

            // Retrieve the associated booking and patient information
            var booking = _dbContext.BookSurgery.FirstOrDefault(b => b.BookingID == bookingID);
            var patient = _dbContext.PatientInfo.FirstOrDefault(p => p.PatientID == booking.PatientID);

            if (patient == null)
            {
                return NotFound("Patient information not found.");
            }

            // Remove the treatment code from the database
            _dbContext.SurgeryTreatmentCode.Remove(surgeryTreatmentCode);
            _dbContext.SaveChanges();

            // Redirect back to the CheckTreatmentCode view with the patient name and surname
            return RedirectToAction("CheckTreatmentCode", new { bookingId = bookingID, name = patient.Name, surname = patient.Surname });
        }

        public IActionResult patientAddmision()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                accountID = 0; 
            }

            var combinedData = (from b in _dbContext.BookSurgery
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join ap in _dbContext.AdmittedPatients on b.BookingID equals ap.BookingID
                                join bed in _dbContext.Bed on ap.BedId equals bed.BedId
                                join w in _dbContext.Ward on bed.WardID equals w.WardId
                                join status in _dbContext.AdmissionStatus on ap.AdmissionStatusID equals status.AdmissionStatusId
                                where b.AccountID == accountID
                                && ap.AdmissionStatusID == 1
                                && bed.Active == true
                                && w.Active == true
                                select new AdmissionsListViewModel
                                {   
                                    AdmittedPatientID = ap.AdmittedPatientID,
                                    BookingID = b.BookingID,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater,
                                    WardName = w.WardName,
                                    BedNumber = bed.Number,
                                    AdmissionStatusDescription = status.Description
                                })
                     .OrderBy(a => a.Name)
                     .ToList();
            var viewModel = new AdmissionsListViewModel
            {
                AllcombinedData = combinedData,

            };

            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
              var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
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
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
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

                    AdmittedPatientID = model.AdmittedPatientID,
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
        public IActionResult VitalsAndHistory(int patientID, string name, string surname)
        {
            var patientVitals = (from pv in _dbContext.PatientVitals
                                 join ap in _dbContext.AdmittedPatients
                                 on pv.PatientID equals ap.PatientID
                                 where pv.PatientID == patientID
                                 select new PatientAllergyViewModel
                                 {
                                     Date = ap.Date,
                                     Time = pv.time,
                                     Height =pv.Height,
                                     Weight= pv.Weight,
                                     SystolicBloodPressure= pv.SystolicBloodPressure,
                                     DiastolicBloodPressure= pv.DiastolicBloodPressure,
                                     HeartRate=  pv.HeartRate,
                                     BloodOxygen=  pv.BloodOxygen,
                                     Respiration= pv.Respiration,
                                     BloodGlucoseLevel= pv.BloodGlucoseLevel,
                                     Temperature =  pv.Temperature
                                    
                                   
                                 }).OrderBy(ap => ap.Date).ToList();


            var allergy = (from pa in _dbContext.PatientAllergy
                           join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                           join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                           where pa.PatientID == patientID
                           select new PatientAllergyViewModel
                           {
                               Name = p.Name,
                               Surname = p.Surname,
                               ActiveIngredientName = ai.ActiveIngredientName
                           })
            .OrderBy(ai => ai.ActiveIngredientName)
            .ToList();


            var conditions = (from pc in _dbContext.PatientConditions
                              join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
                              join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID
                              where pc.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pt.Name,
                                  Surname = pt.Surname,
                                  ConditionName = c.ConditionName // Ensure this property exists in your view model
                              }).OrderBy(c => c.ConditionName).ToList();

            var currentMed = (from pm in _dbContext.patientMedication
                              join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
                              join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
                              where pm.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pi.Name,
                                  Surname = pi.Surname,
                                  MedicationName = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.MedicationName).ToList();
            // Create a view model that holds both lists
            var viewModel = new PatientAllergyViewModel
            {   Allvitals = patientVitals,
                Allallergy = allergy,
                AllConditions = conditions,
                AllCurrentMed = currentMed
            };

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.PatientName = name;
            ViewBag.PatientSurname = surname;
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            return View(viewModel);
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
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }
        public IActionResult PrescriptionMedication(int id)
        {
          
            var combinedData = (from pr in _dbContext.Prescription
                                join ap in _dbContext.AdmittedPatients on pr.AdmittedPatientID equals ap.AdmittedPatientID
                                join p in _dbContext.PatientInfo on ap.PatientID equals p.PatientID
                                join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
                                where pr.PrescriptionID == id
                                orderby p.Name
                                select new PrescriptionMedicationViewModel
                                {
                                    Medid = mi.InstructionsID,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Status = pr.Status,
                                    MedicationName = m.MedicationName,
                                    Quantity = mi.Quantity,
                                    MedicationForm = m.MedicationForm,
                                    Schedule = m.Schedule,
                                    Instructions = mi.Instructions
                                }).ToList();

            

            var patientInfo = _dbContext.PatientInfo
                                        .Where(p => _dbContext.AdmittedPatients
                                                    .Any(ap => ap.PatientID == p.PatientID && _dbContext.Prescription
                                                    .Any(pr => pr.AdmittedPatientID == ap.AdmittedPatientID && pr.PrescriptionID == id)))
                                        .Select(p => new PatientListViewModal
                                        {
                                            Name = p.Name,
                                            Surname = p.Surname,
                                            Gender = p.Gender, // Adjust as necessary
                                        }).FirstOrDefault();

            var patientID = _dbContext.Prescription
                             .Where(pr => pr.PrescriptionID == id)
                             .Join(_dbContext.AdmittedPatients, ap => ap.AdmittedPatientID, bs => bs.AdmittedPatientID, (pr, bs) => bs.PatientID)
                             .FirstOrDefault();

            var patientVitals = (from pv in _dbContext.PatientVitals
                                 join ap in _dbContext.AdmittedPatients
                                 on pv.PatientID equals ap.PatientID
                                 where pv.PatientID == patientID
                                 select new PrescriptionMedicationViewModel
                                 {
                                     Date = ap.Date,
                                     Time = pv.time,
                                     Height = pv.Height,
                                     Weight = pv.Weight,
                                     SystolicBloodPressure = pv.SystolicBloodPressure,
                                     DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                     HeartRate = pv.HeartRate,
                                     BloodOxygen = pv.BloodOxygen,
                                     Respiration = pv.Respiration,
                                     BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                     Temperature = pv.Temperature
                                 }).OrderBy(ap => ap.Date).ToList();

            var allGoodMedications = _dbContext.PharmacyMedication
     .Where(pm => !_dbContext.MedicationActiveIngredient
         .Any(ma => ma.MedicationID == pm.MedicationID && _dbContext.PatientAllergy
         .Any(pa => pa.PatientID == patientID && pa.ActiveingredientID == ma.ActiveingredientID)))
     .Join(_dbContext.Medication, pm => pm.MedicationID, m => m.MedicationID, (pm, m) => new { pm, m }) // Join with Medication to get MedicationName
     .OrderBy(x => x.m.MedicationName)  // Order by MedicationName
     .Select(x => new PrescriptionMedicationViewModel
     {
         MedicationID = x.m.MedicationID,
         MedicationName = x.m.MedicationName  // Use MedicationName from Medication table
     })
     .Distinct()
     .ToList();



            var allMedication = (from pa in _dbContext.PatientAllergy
                                 join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                                 join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                                 join ma in _dbContext.MedicationActiveIngredient on ai.ActiveingredientID equals ma.ActiveingredientID
                                 join pm in _dbContext.PharmacyMedication on ma.MedicationID equals pm.MedicationID // Adjusted join
                                 join m in _dbContext.Medication on pm.MedicationID equals m.MedicationID
                                 where pa.PatientID == patientID
                                 orderby m.MedicationName
                                 select new PrescriptionMedicationViewModel
                                 {
                                     ActiveIngredientName = ai.ActiveIngredientName,
                                     MedicationID = m.MedicationID,
                                     MedicationName = m.MedicationName
                                 }).ToList();

            var allergies = (from pa in _dbContext.PatientAllergy
                             join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                             join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                             where pa.PatientID == patientID
                             select new PatientAllergyViewModel
                             {
                                 Name = p.Name,
                                 Surname = p.Surname,
                                 ActiveIngredientName = ai.ActiveIngredientName
                             }).OrderBy(ai => ai.ActiveIngredientName).ToList();


            var currentMed = (from pm in _dbContext.patientMedication
                              join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
                              join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
                              where pm.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pi.Name,
                                  Surname = pi.Surname,
                                  MedicationName = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.MedicationName).ToList();

            var conditions = (from pc in _dbContext.PatientConditions
                              join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
                              join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID
                              where pc.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pt.Name,
                                  Surname = pt.Surname,
                                  ConditionName = c.ConditionName // Ensure this property exists in your view model
                              }).OrderBy(c => c.ConditionName).ToList();

            var viewModel = new PrescriptionMedicationViewModel
            {
                Allvitals = patientVitals,
                CombinedData = combinedData,
                AllGoodMedications = allGoodMedications,
                AllMedication = allMedication,
                PatientInfo = patientInfo ?? new PatientListViewModal(),
                Allergies = allergies,
                CurrentMedications = currentMed,
                Conditions = conditions// Add this property to your view model
            };
            var today = DateOnly.FromDateTime(DateTime.Today);
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.PrescriptionID = id;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddMedication(MedicationInstructions model)
        {
            if (ModelState.IsValid)
            {
                
                MedicationInstructions newMedicationInstructions = new MedicationInstructions
                {
                    PrescriptionID = model.PrescriptionID,
                    MedicationID = model.MedicationID,
                    Instructions = model.Instructions,
                    Quantity = model.Quantity
             
                };

                _dbContext.MedicationInstructions.Add(newMedicationInstructions);
                _dbContext.SaveChanges();

                return RedirectToAction("PrescriptionMedication", new { id = newMedicationInstructions.PrescriptionID });
            }

            // If validation fails, redisplay the form with errors
            return View("ListSurgery", model);

        }
        [HttpPost]
        public IActionResult DeleteMedication(int Medid, int prescriptionId)
        {
            // Find the medication instruction to delete
            var medicationInstruction = _dbContext.MedicationInstructions
                .FirstOrDefault(mi => mi.InstructionsID == Medid);

            if (medicationInstruction == null)
            {
                // Handle the case where the medication instruction is not found
                return NotFound();
            }

            // Remove the medication instruction from the database
            _dbContext.MedicationInstructions.Remove(medicationInstruction);
            _dbContext.SaveChanges();

            // Redirect back to the list or details page
            return RedirectToAction("PrescriptionMedication", new { id = prescriptionId });
        }

        public async Task<IActionResult> SendPatientEmail(int bookingID)
        {
            // Query to get the combined data
            var combinedData = await (from st in _dbContext.SurgeryTreatmentCode
                                      join t in _dbContext.TreatmentCodes on st.TreatmentCodeID equals t.TreatmentCodeID
                                      join b in _dbContext.BookSurgery on st.BookingID equals b.BookingID
                                      join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                      join a in _dbContext.Accounts on b.AccountID equals a.AccountID
                                      where st.BookingID == bookingID
                                      select new PatientEmailViewModel
                                      {
                                          SurgeryDate = b.SurgeryDate,
                                          SurgeryTime = b.SurgeryTime,
                                          Name = p.Name,
                                          Surname = p.Surname,
                                          Theater = b.Theater,
                                          ContactNumber = p.ContactNumber,
                                          Email = p.Email,
                                          FullName = p.Name + " " + p.Surname,
                                          TreatmentName = t.TreatmentName,
                                          AccountName = a.Name, // Account's Name
                                          AccountSurname = a.Surname // Account's Surname
                                      }).ToListAsync();

            if (combinedData == null || !combinedData.Any())
            {
                // Handle the case where no data is found
                return NotFound();
            }

            // Query to get all treatment names linked to the bookingId
            var treatmentNames = await (from st in _dbContext.SurgeryTreatmentCode
                                        join t in _dbContext.TreatmentCodes on st.TreatmentCodeID equals t.TreatmentCodeID
                                        where st.BookingID == bookingID
                                        select t.TreatmentName)
                                   .ToListAsync();

            // Combine treatment names into a single string
            var treatmentNamesString = string.Join(", ", treatmentNames);

            var firstData = combinedData.First(); // Get the first entry for common fields

            var viewModel = new PatientEmailViewModel
            {
                TreatmentName = treatmentNamesString,
                SurgeryDate = firstData.SurgeryDate,
                SurgeryTime = firstData.SurgeryTime,
                FullName = $"{firstData.Name} {firstData.Surname}",
                Theater = firstData.Theater,
                ContactNumber = firstData.ContactNumber,
                Email = firstData.Email,
                AccountName = firstData.AccountName, // Account's Name
                AccountSurname = firstData.AccountSurname, // Account's Surname
                Notes = string.Empty, // Initial empty notes
                BookingID = bookingID
            };

            // Return the view with the model
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendPatientEmail(int bookingID, string notes)
        {
            var combinedData = await (from st in _dbContext.SurgeryTreatmentCode
                                      join t in _dbContext.TreatmentCodes on st.TreatmentCodeID equals t.TreatmentCodeID
                                      join b in _dbContext.BookSurgery on st.BookingID equals b.BookingID
                                      join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                      join a in _dbContext.Accounts on b.AccountID equals a.AccountID
                                      where st.BookingID == bookingID
                                      select new PatientEmailViewModel
                                      {
                                          SurgeryDate = b.SurgeryDate,
                                          SurgeryTime = b.SurgeryTime,
                                          Name = p.Name,
                                          Surname = p.Surname,
                                          Theater = b.Theater,
                                          ContactNumber = p.ContactNumber,
                                          Email = p.Email,
                                          FullName = p.Name + " " + p.Surname,
                                          TreatmentName = t.TreatmentName,
                                          AccountName = a.Name, // Account's Name
                                          AccountSurname = a.Surname // Account's Surname
                                      }).ToListAsync();

            if (combinedData == null || !combinedData.Any())
            {
                TempData["ErrorMessage"] = "Booking not found or no treatments available.";
                return RedirectToAction("SurgeonHome", "Home");
            }

            var firstData = combinedData.First(); // Get the first entry for common fields

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Day Hospital - Apollo+(Group 9 - 4Year)", "noreply@dayhospital.com"));
            emailMessage.To.Add(new MailboxAddress("Patient", firstData.Email));
            emailMessage.Subject = "Surgery Booking Confirmation";

            var treatmentList = new System.Text.StringBuilder();
            foreach (var item in combinedData)
            {
                treatmentList.AppendLine($"<li>{item.TreatmentName}</li>");
            }

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<h3>User Information</h3>
<p><strong>Full Name:</strong> {firstData.Name} {firstData.Surname}</p>
<p><strong>Contact Details:</strong> {firstData.ContactNumber} {firstData.Email}</p>
<h3>You have been booked for the following surgery by Dr.{firstData.AccountName} {firstData.AccountSurname}</h3>
<p><strong>Theater:</strong> {firstData.Theater}</p>
<p><strong>Surgery Time:</strong> {firstData.SurgeryTime}</p>
<h3>Treatments</h3>
<ul>
    {treatmentList}
</ul>
<h3>Notes</h3>
<p>{notes}</p>
<p>Kind Regards,<br/>Apollo+</p>"
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("jansen.ronaldocullen@gmail.com", "xqqx kiox hcgm xvmr");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                TempData["SuccessMessage"] = "Email sent successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error sending email: {ex.Message}";
            }

            return RedirectToAction("SurgeonHome", "Home");
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
                              join ap in _dbContext.AdmittedPatients
            on p.PatientID equals ap.PatientID
                              join pr in _dbContext.Prescription
                              on ap.AdmittedPatientID equals pr.AdmittedPatientID
                              where pr.Status == "Prescribed" && pr.AccountID == accountID
                              select new PrescriptionListViewModal

                              {   PrescriptionID = pr.PrescriptionID,
                                  Name = p.Name,
                                  Surname=p.Surname,
                                  DateGiven = pr.DateGiven,
                                  Urgency = pr.Urgency,
                                  Take = pr.Take,
                                  Status = pr.Status
                              }).OrderBy(a => a.Name).ToList();

            var PrescribedDispensed = (from p in _dbContext.PatientInfo
                                       join ap in _dbContext.AdmittedPatients
                                       on p.PatientID equals ap.PatientID
                                       join pr in _dbContext.Prescription
                                       on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                       join rs in _dbContext.DispensedScriptsModel
                                       on pr.PrescriptionID equals rs.PrescriptionID
                                       join a in _dbContext.Accounts
                                       on rs.AccountID equals a.AccountID
                                       where pr.Status == "Dispensed" && pr.AccountID == accountID
                                       orderby p.Name
                                       select new PrescriptionListViewModal
                                       {
                                           PrescriptionID = pr.PrescriptionID,
                                           PatientName = p.Name,
                                           PatientSurname = p.Surname,
                                           DateGiven = pr.DateGiven,
                                           Urgency = pr.Urgency,
                                           Take = pr.Take,
                                           Status = pr.Status,
                                           AccountName = a.Name,
                                           AccountSurname = a.Surname
                                       }).ToList();

            var PrescribedRejected = (from p in _dbContext.PatientInfo
                                      join ap in _dbContext.AdmittedPatients
                                      on p.PatientID equals ap.PatientID
                                      join pr in _dbContext.Prescription
                                      on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                      join rs in _dbContext.RejectScriptModel
                                      on pr.PrescriptionID equals rs.PrescriptionID
                                      join a in _dbContext.Accounts
                                      on rs.AccountID equals a.AccountID
                                      where pr.Status == "Rejected" && pr.AccountID == accountID
                                      orderby p.Name
                                      select new PrescriptionListViewModal
                                      {
                                          PrescriptionID = pr.PrescriptionID,
                                          PatientName = p.Name,
                                          PatientSurname = p.Surname,
                                          DateGiven = pr.DateGiven,
                                          Urgency = pr.Urgency,
                                          Take = pr.Take,
                                          Status = pr.Status,
                                          RejectionReason = rs.RejectionReason,
                                          AccountName = a.Name,
                                          AccountSurname = a.Surname
                                      }).ToList();

            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribed = Prescribed,
                AllPrescribedDispensed = PrescribedDispensed,
                AllPrescribedRejected = PrescribedRejected
            };
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult PrescriptionDetails(int id)
        {
            var prescriptionDetails = (from pr in _dbContext.Prescription
                                       join ap in _dbContext.AdmittedPatients on pr.AdmittedPatientID equals ap.AdmittedPatientID
                                       join p in _dbContext.PatientInfo on ap.PatientID equals p.PatientID
                                       join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                       join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
                                       where pr.PrescriptionID == id
                                       orderby p.Name
                                       select new PrescriptionMedicationViewModel
                                       {
                                           Medid = mi.InstructionsID,
                                           Name = p.Name,
                                           Surname = p.Surname,
                                           DateGiven = pr.DateGiven,
                                           Status = pr.Status,
                                           MedicationName = m.MedicationName,
                                           Quantity = mi.Quantity,
                                           MedicationForm = m.MedicationForm,
                                           Schedule = m.Schedule,
                                           Instructions = mi.Instructions
                                       }).ToList();

            return Json(prescriptionDetails);  // Return data as JSON
        }

        public IActionResult MedicationInteraction()
        {
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
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
            var today = DateOnly.FromDateTime(DateTime.Today);

            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View();
        }

        public IActionResult ChangeMedication(int id)
        {

            var combinedData = (from pr in _dbContext.Prescription
                                join ap in _dbContext.AdmittedPatients on pr.AdmittedPatientID equals ap.AdmittedPatientID
                                join p in _dbContext.PatientInfo on ap.PatientID equals p.PatientID
                                join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
                                where pr.PrescriptionID == id
                                orderby p.Name
                                select new PrescriptionMedicationViewModel
                                {
                                    Medid = mi.InstructionsID,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Status = pr.Status,
                                    MedicationName = m.MedicationName,
                                    Quantity = mi.Quantity,
                                    MedicationForm = m.MedicationForm,
                                    Schedule = m.Schedule,
                                    Instructions = mi.Instructions
                                }).ToList();



            var patientInfo = _dbContext.PatientInfo
                                        .Where(p => _dbContext.AdmittedPatients
                                                    .Any(ap => ap.PatientID == p.PatientID && _dbContext.Prescription
                                                    .Any(pr => pr.AdmittedPatientID == ap.AdmittedPatientID && pr.PrescriptionID == id)))
                                        .Select(p => new PatientListViewModal
                                        {
                                            Name = p.Name,
                                            Surname = p.Surname,
                                            Gender = p.Gender, // Adjust as necessary
                                        }).FirstOrDefault();

            var patientID = _dbContext.Prescription
                             .Where(pr => pr.PrescriptionID == id)
                             .Join(_dbContext.AdmittedPatients, ap => ap.AdmittedPatientID, bs => bs.AdmittedPatientID, (pr, bs) => bs.PatientID)
                             .FirstOrDefault();

            var patientVitals = (from pv in _dbContext.PatientVitals
                                 join ap in _dbContext.AdmittedPatients
                                 on pv.PatientID equals ap.PatientID
                                 where pv.PatientID == patientID
                                 select new PrescriptionMedicationViewModel
                                 {
                                     Date = ap.Date,
                                     Time = pv.time,
                                     Height = pv.Height,
                                     Weight = pv.Weight,
                                     SystolicBloodPressure = pv.SystolicBloodPressure,
                                     DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                     HeartRate = pv.HeartRate,
                                     BloodOxygen = pv.BloodOxygen,
                                     Respiration = pv.Respiration,
                                     BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                     Temperature = pv.Temperature
                                 }).OrderBy(ap => ap.Date).ToList();

            var allGoodMedications = _dbContext.PharmacyMedication
     .Where(pm => !_dbContext.MedicationActiveIngredient
         .Any(ma => ma.MedicationID == pm.MedicationID && _dbContext.PatientAllergy
         .Any(pa => pa.PatientID == patientID && pa.ActiveingredientID == ma.ActiveingredientID)))
     .Join(_dbContext.Medication, pm => pm.MedicationID, m => m.MedicationID, (pm, m) => new { pm, m }) // Join with Medication to get MedicationName
     .OrderBy(x => x.m.MedicationName)  // Order by MedicationName
     .Select(x => new PrescriptionMedicationViewModel
     {
         MedicationID = x.m.MedicationID,
         MedicationName = x.m.MedicationName  // Use MedicationName from Medication table
     })
     .Distinct()
     .ToList();



            var allMedication = (from pa in _dbContext.PatientAllergy
                                 join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                                 join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                                 join ma in _dbContext.MedicationActiveIngredient on ai.ActiveingredientID equals ma.ActiveingredientID
                                 join pm in _dbContext.PharmacyMedication on ma.MedicationID equals pm.MedicationID // Adjusted join
                                 join m in _dbContext.Medication on pm.MedicationID equals m.MedicationID
                                 where pa.PatientID == patientID
                                 orderby m.MedicationName
                                 select new PrescriptionMedicationViewModel
                                 {
                                     ActiveIngredientName = ai.ActiveIngredientName,
                                     MedicationID = m.MedicationID,
                                     MedicationName = m.MedicationName
                                 }).ToList();

            var allergies = (from pa in _dbContext.PatientAllergy
                             join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                             join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                             where pa.PatientID == patientID
                             select new PatientAllergyViewModel
                             {
                                 Name = p.Name,
                                 Surname = p.Surname,
                                 ActiveIngredientName = ai.ActiveIngredientName
                             }).OrderBy(ai => ai.ActiveIngredientName).ToList();


            var currentMed = (from pm in _dbContext.patientMedication
                              join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
                              join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
                              where pm.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pi.Name,
                                  Surname = pi.Surname,
                                  MedicationName = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.MedicationName).ToList();

            var conditions = (from pc in _dbContext.PatientConditions
                              join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
                              join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID
                              where pc.PatientID == patientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pt.Name,
                                  Surname = pt.Surname,
                                  ConditionName = c.ConditionName // Ensure this property exists in your view model
                              }).OrderBy(c => c.ConditionName).ToList();

            var viewModel = new PrescriptionMedicationViewModel
            {
                Allvitals = patientVitals,
                CombinedData = combinedData,
                AllGoodMedications = allGoodMedications,
                AllMedication = allMedication,
                PatientInfo = patientInfo ?? new PatientListViewModal(),
                Allergies = allergies,
                CurrentMedications = currentMed,
                Conditions = conditions// Add this property to your view model
            };
            var today = DateOnly.FromDateTime(DateTime.Today);
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var surgeryCount = _dbContext.BookSurgery
                .Where(bs => bs.AccountID.ToString() == accountID && bs.SurgeryDate == today)
                .Count();


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID.ToString() == accountID
                                   select pr).Count();

            var dispensedCount = (from p in _dbContext.PatientInfo
                                  join ap in _dbContext.AdmittedPatients
                                  on p.PatientID equals ap.PatientID
                                  join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                  where pr.Status == "Dispensed" && pr.AccountID.ToString() == accountID
                                  select pr).Count();

            var rejectedCount = (from p in _dbContext.PatientInfo
                                 join ap in _dbContext.AdmittedPatients
                                 on p.PatientID equals ap.PatientID
                                 join pr in _dbContext.Prescription
                                  on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                 where pr.Status == "Rejected" && pr.AccountID.ToString() == accountID
                                 select pr).Count();

            // Pass the prescribed count to the view
            ViewBag.SurgeryCount = surgeryCount;
            ViewBag.PrescribedCount = prescribedCount;
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.PrescriptionID = id;
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
