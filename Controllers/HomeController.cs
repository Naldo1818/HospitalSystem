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
                        return RedirectToAction("SurgeonHome", new { AccountID });
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
        public IActionResult SurgeonHome(int accountId)
        {
            // Try to get data from session first
           // var accountID = HttpContext.Session.GetString("UserAccountId");
            //var name = HttpContext.Session.GetString("UserName");
            //var surname = HttpContext.Session.GetString("UserSurname");
            //var email = HttpContext.Session.GetString("UserEmail");

            // Retrieve from database if not in session
            var surgeon = _dbContext.Accounts
                .Where(a => a.AccountID == accountId)
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

            //if (!string.IsNullOrEmpty(accountID) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(email))
            //{
            //    // Use existing session data
           ViewBag.AccountId = surgeon.AccountID;
            //    ViewBag.UserName = name;
            //    ViewBag.UserSurname = surname;
            //    ViewBag.UserEmail = email;
            //}
            //else
            //{
               

                ViewBag.UserName = surgeon.AccountID.ToString();
                ViewBag.UserName = surgeon.Name;
                ViewBag.UserSurname = surgeon.Surname;
                ViewBag.UserEmail = surgeon.Email;
            //}

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
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }

            var combinedData = (from b in _dbContext.BookSurgery
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                where b.AccountID == accountID
                                select new AdmissionsListViewModel
                                {
                                    BookingID = b.BookingID,
                                    PatientID = b.PatientID,
                                    AccountID = b.AccountID,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater
                                }).OrderBy(a => a.Name).ToList();

            var viewModel = new AdmissionsListViewModel
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
        public IActionResult VitalsAndHistory(int patientID)
        {
            var allergy = (from pa in _dbContext.PatientAllergy
                           join p in _dbContext.PatientInfo on pa.patientAllergyID equals p.PatientID
                           join ai in _dbContext.Activeingredient
                               on pa.ActiveingredientID equals ai.ActiveingredientID
                           where pa.patientAllergyID == patientID
                           select new PatientAllergyViewModel
                           {
                               Name = p.Name,
                               Surname = p.Surname,
                               ActiveIngredientName = ai.ActiveIngredientName
                           }).OrderBy(ai => ai.ActiveIngredientName).ToList();

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
                              join cm in _dbContext.CurrentMedication on pm.CurrentID equals cm.CurrentId
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
            {
                Allallergy = allergy,
                AllConditions = conditions,
                AllCurrentMed = currentMed
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

        public IActionResult SendPatientEmail(int id)
        {
            var prescriptionDetails = (from p in _dbContext.Prescription
                                       join b in _dbContext.BookSurgery on p.BookingID equals b.BookingID
                                       join pa in _dbContext.PatientInfo on b.PatientID equals pa.PatientID
                                       where p.PrescriptionID == id
                                       select new
                                       {
                                           b.SurgeryTime,
                                           b.SurgeryDate,
                                           b.Theater,
                                           pa.Name,
                                           pa.Surname,
                                           pa.ContactNumber,
                                           pa.Email
                                       }).FirstOrDefault();

            // Prepare the view model
            var viewModel = new PatientEmailViewModel
            {
                SurgeryDate = prescriptionDetails.SurgeryDate,
                SurgeryTime = prescriptionDetails.SurgeryTime,
                FullName = $"{prescriptionDetails.Name} {prescriptionDetails.Surname}",

                Theater = prescriptionDetails.Theater,
                ContactNumber = prescriptionDetails.ContactNumber,
                Email = prescriptionDetails.Email, // Only for example; usually, passwords are not shared like this
                Notes = string.Empty // Initial empty notes
            };

            return View(viewModel); // Return the view with the model

        }

        [HttpPost]
        public async Task<IActionResult> SendPatientEmail(int id, string notes)
        {

            var prescriptionDetails = (from p in _dbContext.Prescription
                                       join b in _dbContext.BookSurgery on p.BookingID equals b.BookingID
                                       join pa in _dbContext.PatientInfo on b.PatientID equals pa.PatientID
                                       where p.PrescriptionID == id
                                       select new
                                       {
                                           b.SurgeryTime,
                                           b.SurgeryDate,
                                           b.Theater,
                                           pa.Name,
                                           pa.Surname,
                                           pa.ContactNumber,
                                           pa.Email
                                       }).FirstOrDefault();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Day Hospital -  Apollo+(Group 9 - 4Year)", "noreply@dayhospital.com"));
            emailMessage.To.Add(new MailboxAddress("Patient", prescriptionDetails.Email));
            emailMessage.Subject = "User Added";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
         <h3>User Information</h3>
         <p><strong>Name:</strong> {prescriptionDetails.Name} {prescriptionDetails.Surname}</p>
         <p><strong>Contact Details:</strong> {prescriptionDetails.ContactNumber} {prescriptionDetails.Email}</p>
         <h3>You have been Booked for the following surgery</h3>
</n>
         <p><strong>Theater:</strong> {prescriptionDetails.Theater}</p>
         <p><strong>SurgeryTime:</strong> {prescriptionDetails.SurgeryTime}</p>        
</n>
         <h3>Notes</h3>
         <p>{notes}</p>
</n> 
          Kind Regards 
          Apollo+"""
            };

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                client.Authenticate("jansen.ronaldocullen@gmail.com", "xqqx kiox hcgm xvmr");
                await client.SendAsync(emailMessage);
                client.Disconnect(true);

            }

            TempData["SuccessMessage"] = "Email sent successfully.";
            return RedirectToAction("AdminHome", "Admin");
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
                                
                                  Name = p.Name,
                                  Surname=p.Surname,
                                  DateGiven = pr.DateGiven,
                                  Urgency = pr.Urgency,
                                  Take = pr.Take,
                                  Status = pr.Status
                              }).OrderBy(a => a.Name).ToList();

            var PrescribedDispensed = (from p in _dbContext.PatientInfo
                                       join bs in _dbContext.BookSurgery
                                       on p.PatientID equals bs.PatientID
                                       join pr in _dbContext.Prescription
                                       on bs.BookingID equals pr.BookingID
                                       join rs in _dbContext.DispensedScriptsModel
                                       on pr.PrescriptionID equals rs.PrescriptionID
                                       join a in _dbContext.Accounts
                                       on rs.AccountID equals a.AccountID
                                       where pr.Status == "Dispensed"
                                       orderby p.Name
                                       select new PrescriptionListViewModal
                                       {
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
                                      join bs in _dbContext.BookSurgery
                                      on p.PatientID equals bs.PatientID
                                      join pr in _dbContext.Prescription
                                      on bs.BookingID equals pr.BookingID
                                      join rs in _dbContext.RejectScriptModel
                                      on pr.PrescriptionID equals rs.PrescriptionID
                                      join a in _dbContext.Accounts
                                      on rs.AccountID equals a.AccountID
                                      where pr.Status == "Rejected"
                                      orderby p.Name
                                      select new PrescriptionListViewModal
                                      {
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
