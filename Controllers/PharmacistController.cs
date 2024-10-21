using DEMO.Data;
using DEMO.Models;
using DEMO.ViewModels;
using DEMO.Data.Migrations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MimeKit;
using MailKit.Net.Smtp;
using Newtonsoft.Json;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using DEMO.Models.PharmacistModels;
using System;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using static DEMO.Controllers.PharmacistController;




namespace DEMO.Controllers
{
    public class PharmacistController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PharmacistController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public ActionResult RejectScript()
        {

            return View();
        }

        [HttpPost]
        public ActionResult RejectScript(int id, string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                return BadRequest("Reason is required.");
            }

            // Your logic to save the rejection reason in the database
            var prescription = _dbContext.RejectScriptModel.Find(id);
            if (prescription != null)
            {
                prescription.RejectionReason = reason;
                prescription.RejectionID = id;
                _dbContext.SaveChanges();
                return Json(new { success = true });
            }

            return NotFound();
        }


        public IActionResult StockOrderPage()
        {
            // Query to get the medications that need to be reordered
            var combinedData = (from pm in _dbContext.PharmacyMedication
                                join m in _dbContext.Medication
                                on pm.MedicationID equals m.MedicationID
                                //where pm.StockonHand <= pm.ReorderLevel
                                select new PharmacistStockOrderViewModel
                                {
                                    MedicationID = m.MedicationID,
                                    MedicationName = m.MedicationName,
                                    MedicationForm = m.MedicationForm,
                                    Schedule = m.Schedule,
                                    StockonHand = pm.StockonHand,
                                    ReorderLevel = pm.ReorderLevel
                                    
                                }).ToList();

            var viewModel = new PharmacistStockOrderViewModel
            {
                PharmacistStockOrders = combinedData
            };

            // Pass the list of data to the view
            return View(viewModel);
        }
        public class OrderItem
        {
            public int MedicationId { get; set; }
            public int Quantity { get; set; }
        }

        public class OrderData
        {
            public List<OrderItem> Orders { get; set; }
            public string Notes { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> StockOrderPage(string notes)
        {
            var PharmacistName = HttpContext.Session.GetString("UserName");
            var PharmacistSurname = HttpContext.Session.GetString("UserSurname");
            var PharmacistEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var medicationsToReorder = await (from pm in _dbContext.PharmacyMedication
                                              join m in _dbContext.Medication
                                              on pm.MedicationID equals m.MedicationID
                                              select new
                                              {
                                                  MedicationName = m.MedicationName,
                                                  MedicationForm = m.MedicationForm,
                                                  Schedule = m.Schedule,
                                                  StockonHand = pm.StockonHand,
                                                  ReorderLevel = pm.ReorderLevel
                                              })
                                              .ToListAsync();

            if (!medicationsToReorder.Any())
            {
                return BadRequest("No medications need to be reordered.");
            }

            // Generate image
            using (var bitmap = new Bitmap(800, 600))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                using (var font = new Font("Arial", 12))
                {
                    int y = 10;
                    graphics.DrawString("Stock Order Details", new Font("Arial", 16, FontStyle.Bold), Brushes.Black, 10, y);
                    y += 30;

                    foreach (var item in medicationsToReorder)
                    {
                        graphics.DrawString($"Medication: {item.MedicationName}", font, Brushes.Black, 10, y);
                        y += 20;
                        graphics.DrawString($"Form: {item.MedicationForm}", font, Brushes.Black, 10, y);
                        y += 20;
                        graphics.DrawString($"Schedule: {item.Schedule}", font, Brushes.Black, 10, y);
                        y += 20;
                        graphics.DrawString($"Stock on Hand: {item.StockonHand}", font, Brushes.Black, 10, y);
                        y += 20;
                        graphics.DrawString($"Reorder Level: {item.ReorderLevel}", font, Brushes.Black, 10, y);
                        y += 30;
                    }

                    graphics.DrawString($"Notes: {notes}", font, Brushes.Black, 10, y);
                    y += 30;
                    graphics.DrawString($"Kind Regards,", font, Brushes.Black, 10, y);
                    y += 20;
                    graphics.DrawString($"{PharmacistName} {PharmacistSurname}", font, Brushes.Black, 10, y);
                }

                // Save image to memory stream
                using (var ms = new MemoryStream())
                {
                    bitmap.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    // Create email message
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress("Day Hospital - Apollo+(Group 9 - 4Year)", PharmacistEmail));
                    emailMessage.To.Add(new MailboxAddress("Purchasing Manager", "sam12mensah@gmail.com"));
                    emailMessage.Subject = "Stock Order Request";

                    var builder = new BodyBuilder();
                    builder.Attachments.Add("StockOrder.png", ms.ToArray());
                    emailMessage.Body = builder.ToMessageBody();

                    // Send email
                    try
                    {
                        using (var client = new MailKit.Net.Smtp.SmtpClient())
                        {
                            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                            await client.AuthenticateAsync("sam12mensah@gmail.com", "xqqx kiox hcgm xvmr");
                            await client.SendAsync(emailMessage);
                        }

                        return Ok("Email sent successfully");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Error sending email: {ex.Message}");
                    }
                }
            }
        }







        public IActionResult ViewAllPrescriptions()
        {
            

            var combinedData = (from prescription in _dbContext.Prescription

                                join ap in _dbContext.AdmittedPatients
                                on prescription.AdmittedPatientID equals ap.AdmittedPatientID// Assuming AccountID is linked to PatientID

                                join pi in _dbContext.PatientInfo

                                on ap.PatientID equals pi.PatientID




                                join account in _dbContext.Accounts
                                on prescription.AccountID equals account.AccountID


                                select new ViewActivePrescriptionsModel


                                {
                                    // Prescription fields
                                    PrescriptionID = prescription.PrescriptionID,
                                    SurgeonName = account.Name,
                                    SurgeonSurname = account.Surname,
                                    DateGiven = prescription.DateGiven,
                                    Urgency = prescription.Urgency,
                                    Status = prescription.Status,

                                    // Medication fields


                                    // MedicationInstructions fields


                                    // PatientInfo fields
                                    Name = pi.Name,
                                    Surname = pi.Surname,



                                })

                                 .ToList();



            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);
        }




        // GET: AddMedication

        public IActionResult AddMedication()
        {
            var combinedData = (from m in _dbContext.Medication
                                join pm in _dbContext.PharmacyMedication
                                on m.MedicationID equals pm.MedicationID
                                join pmm in _dbContext.PharmacyMedicationModel
                                on m.MedicationID equals pmm.MedicationID






                                select new PharmacyMedicationViewModel


                                {
                                    MedicationName = m.MedicationName,
                                    MedicationForm = m.MedicationForm,
                                    Schedule = m.Schedule,
                                    StockonHand = pm.StockonHand,
                                    ReorderLevel = pm.ReorderLevel,






                                })

                                 .ToList();


            // Fetch medication schedules
            var medSchedules = _dbContext.Medication
                              .Select(m => m.Schedule)
                              .Distinct()
                              .OrderBy(schedule => schedule) // Order by ascending
                              .ToList();




            //Fetch Active Ingredients




            var df = _dbContext.Medication
               .Select(m => m.MedicationForm)
               .Distinct()
               .ToString();



            var schedule = _dbContext.Medication
               .Select(m => m.Schedule)
               .Distinct()
               .ToString();

            var stockonhand = _dbContext.PharmacyMedication
                .Select(m => m.StockonHand)
                .Distinct()
                .ToString();




            var activeingredientslist = _dbContext.Activeingredient
                                   .Select(m => m.ActiveIngredientName)
                                   .Distinct()
                                   .OrderBy(name => name) // Order in alphabetical order
                                   .ToList();







            var active = _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .ToString();



            var dfdropdown = _dbContext.Medication
                            .Select(m => m.MedicationForm)
                            .Distinct()
                            .OrderBy(form => form) // Order by ascending
                            .ToList();




            // Create a ViewModel to hold the data
            var viewModel = new PharmacyMedicationViewModel
            {


                MedicationForm = df,

                Schedules = medSchedules,
                DosageForms = dfdropdown,
                ActiveIngredientsDropdown = activeingredientslist,



                combinedinfo = combinedData,














            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMedication(PharmacyMedicationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Medication entity
                Medication med = new Medication
                {
                    MedicationName = model.MedicationName,
                    MedicationForm = model.MedicationForm,
                    Schedule = model.Schedule,
                };

                // Add and save the new Medication entity to the database
                _dbContext.Medication.Add(med);
                _dbContext.SaveChanges();

                // Fetch the Medication entity just saved
                var medEntity = _dbContext.Medication
                                .FirstOrDefault(m => m.MedicationName == model.MedicationName);

                // Ensure medication was successfully retrieved before proceeding
                if (medEntity != null)
                {
                    // Create a new PharmMedModel entity and link to the saved Medication
                    PharmMedModel pharmMed = new PharmMedModel
                    {
                        MedicationID = medEntity.MedicationID,
                        StockonHand = model.StockonHand,
                        ReorderLevel = model.ReorderLevel,
                    };

                    // Add and save the PharmMedModel entity to the database
                    _dbContext.PharmacyMedication.Add(pharmMed);
                    _dbContext.SaveChanges();

                    return RedirectToAction("AddMedication");
                }
                else
                {
                    // Handle error: Medication retrieval failed
                    ModelState.AddModelError("", "Error saving medication.");
                }


            }

            // Return the view with validation errors if any
            // Fetch schedules

            //model.Schedules=sched



            var schedules = _dbContext.Medication
                              .Select(m => m.Schedule)
                              .Distinct()
                              .OrderBy(schedule => schedule) // Order by ascending
                              .ToList();

            var dosageforms = _dbContext.Medication
                .Select(m => m.MedicationForm)
                .Distinct()
                .OrderBy(form => form)
                .ToList();



            var activeingredientslist = _dbContext.Activeingredient
                                 .Select(m => m.ActiveIngredientName)
                                 .Distinct()
                                 .OrderBy(name => name) // Order in alphabetical order
                                 .ToList();







            var active = _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .ToString();

            model.ActiveIngredientsDropdown = activeingredientslist;
            model.Schedules = schedules;
            model.DosageForms = dosageforms;
            return View(model);
        }








        public IActionResult ViewAddedMedication()
        {
            var combinedData = (from m in _dbContext.Medication
                                join pm in _dbContext.PharmacyMedication
                                on m.MedicationID equals pm.MedicationID







                                select new PharmacyMedicationViewModel


                                {
                                    MedicationName = m.MedicationName,
                                    MedicationForm = m.MedicationForm,
                                    Schedule = m.Schedule,
                                    StockonHand = pm.StockonHand,
                                    ReorderLevel = pm.ReorderLevel,






                                })

                                .ToList();

            var viewModel = new PharmacyMedicationViewModel
            {




                combinedinfo = combinedData,





            };
            return View(viewModel);
        }





























        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PharmacistHomePage(int accountId)
        {
            var pharmacist = _dbContext.Accounts
                .Where(a => a.AccountID == accountId)
                .Select(a => new PharmacistView
                {
                    AccountID = a.AccountID,
                    Name = a.Name,
                    Surname = a.Surname,
                    Email = a.Email
                })
                .SingleOrDefault();

            if (pharmacist == null)
            {
                return NotFound();
            }

            // Store user data in session
            HttpContext.Session.SetString("UserAccountId", pharmacist.AccountID.ToString());
            HttpContext.Session.SetString("UserName", pharmacist.Name);
            HttpContext.Session.SetString("UserSurname", pharmacist.Surname);
            HttpContext.Session.SetString("UserEmail", pharmacist.Email);





            ViewBag.UserName = pharmacist.AccountID.ToString();
            ViewBag.UserName = pharmacist.Name;
            ViewBag.UserSurname = pharmacist.Surname;
            ViewBag.UserEmail = pharmacist.Email;
            //}

            return View();
        }

        public IActionResult ManageMedicationStockPage()
        {
            return View();
        }


        public IActionResult ViewDailyUsageReport()
        {
            return View();
        }




        [HttpGet]

        public async Task<IActionResult> ViewSpecificPrescription(int pid)
        {
            // Retrieve the specific prescription using the provided id
            var prescription = await _dbContext.Prescription.FindAsync(pid);

            if (prescription == null)
            {
                return NotFound();
            }







            var patientConditions = await _dbContext.Condition
    .Select(m => m.ConditionName)
    .Distinct()
    .OrderBy(name => name) // Sort in ascending order
    .ToListAsync();

            var currentMeds = await _dbContext.Medication
                .Select(m => m.MedicationName)
                .Distinct()
                .OrderBy(name => name) // Sort in ascending order
                .ToListAsync();

            var allAllergies = await _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .OrderBy(name => name) // Sort in ascending order
                .ToListAsync();




            // Get the specific prescription data along with related patient information
            var alldata = await (from p in _dbContext.Prescription
                                 join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                 join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                 join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                 join account in _dbContext.Accounts on p.AccountID equals account.AccountID

                                 join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                 join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                 where p.PrescriptionID == pid // Filter by prescription ID
                                 select new PharmacistViewScriptModel
                                 {
                                     Urgency = p.Urgency,
                                     PrescriptionID = p.PrescriptionID,
                                     AdmittedPatientID = p.AdmittedPatientID,
                                     patientname = pi.Name,
                                     patientsurname = pi.Surname,
                                     Status = p.Status,
                                     Height = ap.Height,
                                     Weight = ap.Weight,
                                     SystolicBloodPressure = pv.SystolicBloodPressure,
                                     DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                     HeartRate = pv.HeartRate,
                                     BloodOxygen = pv.BloodOxygen,
                                     Respiration = pv.Respiration,
                                     BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                     Temperature = pv.Temperature,
                                     SurgeonName = account.Name,
                                     SurgeonSurname = account.Surname,


                                     qty = mi.Quantity,
                                     Instructions = mi.Instructions,
                                     medication = m.MedicationName,
                                 })
                                  .ToListAsync();

            // Create the view model with combined data and additional information
            var viewModel = new PharmacistViewScriptModel
            {
                combinedData = alldata, // Ensure property names match your model's definition
                Allallergy = allAllergies,
                AllConditions = patientConditions,
                AllCurrentMed = currentMeds,
            };

            return View(viewModel);
        }












        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewSpecificPrescription()
        {
            

           var prescription = _dbContext.Prescription.Select(m=>m.Status).ToString();

            

                string newstatus = "Prescribed";

            prescription = newstatus;

            _dbContext.Add(prescription);
            _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(ViewAllActivePrescriptionsPage)); // Redirect to the list of prescriptions
        }










        //           








        //         var accountID = HttpContext.Session.GetString("UserAccountId");
        //         var userName = HttpContext.Session.GetString("UserName");
        //         var userSurname = HttpContext.Session.GetString("UserSurname");
        //         var userEmail = HttpContext.Session.GetString("UserEmail");
        //         var today = DateOnly.FromDateTime(DateTime.Today);












        public ActionResult DispenseMedication()
        {

            return View();
        }





        public ActionResult MedicationDispensed()
        {

            return View();
        }

        public ActionResult PrescriptionRejected()
        {

            return View();
        }



        public ActionResult MedicationAdded()
        {

            return View();
        }



        public ActionResult OrderStock()
        {

            return View();
        }


        public ActionResult StockOrdered()
        {

            return View();
        }


        public ActionResult ViewSpecificMedicalHistory()
        {
            return View();
        }


        public ActionResult ViewSpecificVitalsHistory()
        {
            return View();
        }

        public ActionResult ViewAllOrders()
        {
            return View();
        }



        public ActionResult ConfirmScriptRejectionPage()
        {
            return View();
        }


        public ActionResult IncomingStockPage()
        {
            return View();
        }


        public ActionResult NewCurrentLevelPage()
        {
            return View();
        }


        public ActionResult CurrentLevelUpdatedPage()
        {
            return View();
        }


        public ActionResult AddActiveIngredientsPage()
        {
            return View();

        }


        public ActionResult ActiveIngredientsAdded()
        {
            return View();

        }


        public ActionResult ViewActiveIngredientsPage()
        {
            return View();

        }


        public ActionResult ViewSpecificActiveIngredients()
        {
            return View();

        }

        public ActionResult ViewCurrentLevels()
        {
            return View();
        }






        public IActionResult ViewAllActivePrescriptionsPage()

        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }

            var combinedData = (from prescription in _dbContext.Prescription
                                    //join medicationInstruction in _dbContext.MedicationInstructions
                                    //on prescription.PrescriptionID equals medicationInstruction.PrescriptionID

                                    //join medication in _dbContext.Medication
                                    //on medicationInstruction.MedicationID equals medication.MedicationID

                                join ap in _dbContext.AdmittedPatients
                                on prescription.AdmittedPatientID equals ap.AdmittedPatientID// Assuming AccountID is linked to PatientID

                                join pi in _dbContext.PatientInfo

                                on ap.PatientID equals pi.PatientID




                                join account in _dbContext.Accounts
                                on prescription.AccountID equals account.AccountID

                                where prescription.Status == "Prescribed"

                                select new ViewActivePrescriptionsModel


                                {
                                    // Prescription fields

                                    SurgeonName = account.Name,
                                    SurgeonSurname = account.Surname,
                                    DateGiven = prescription.DateGiven,
                                    Urgency = prescription.Urgency,
                                    Status = prescription.Status,
                                    AdmittedPatientID = ap.AdmittedPatientID,
                                    PrescriptionID = prescription.PrescriptionID,

                                    // Medication fields


                                    // MedicationInstructions fields


                                    // PatientInfo fields
                                    Name = pi.Name,
                                    Surname = pi.Surname,



                                })
                                .OrderByDescending(item => item.Urgency == "Yes")
                                .ToList();



            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);



        }



























        public ActionResult ViewSpecificPrescriptionDisabled()
        {
            return View();
        }





        public IActionResult Report()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }

            var combinedData = (from p in _dbContext.PatientInfo
                                join pp in _dbContext.AdmittedPatients on p.PatientID equals pp.PatientID
                                join prp in _dbContext.Prescription on pp.AdmittedPatientID equals prp.AdmittedPatientID
                                join pharmacist in _dbContext.Accounts on prp.AccountID equals pharmacist.AccountID


                                where prp.AccountID == accountID
                                orderby prp.DateGiven

                                select new PharmacistReportViewModel
                                {
                                    PrescriptionDate = prp.DateGiven,
                                    Patient = p.Name + " " + p.Surname,
                                    Surgeon = pharmacist.Name + " " + pharmacist.Surname,
                                    status = prp.Status,



                                }).ToList();

            var viewModel = new PharmacistReportViewModel
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
            ViewBag.DispensedCount = dispensedCount;
            ViewBag.RejectedCount = rejectedCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }


    }
}















