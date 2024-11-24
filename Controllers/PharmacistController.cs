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
using DEMO.Models.NurseModels;
using System.Security.Cryptography;




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
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                       ).Count();

            ViewBag.Count = count;


            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;


            //var lowstockcount = (from stock in _dbContext.)

            // Query to get the medications that need to be reordered
            var combinedData = (from pm in _dbContext.PharmacyMedication
                                join m in _dbContext.Medication
                                on pm.MedicationID equals m.MedicationID
                                where pm.StockonHand <= pm.ReorderLevel
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOrderPage(PharmacistStockOrderViewModel model)
        {
            var PharmacistName = HttpContext.Session.GetString("UserName");
            var PharmacistSurname = HttpContext.Session.GetString("UserSurname");
            var PharmacistEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);


            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                       ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;


            var medicationsToReorder =  (from pm in _dbContext.PharmacyMedication
                                              join m in _dbContext.Medication
                                              on pm.MedicationID equals m.MedicationID
                                              where pm.StockonHand <= pm.ReorderLevel
                                              select new
                                              {
                                                  MedicationName = m.MedicationName,
                                                  MedicationForm = m.MedicationForm,
                                                  Schedule = m.Schedule,
                                                  StockonHand = pm.StockonHand,
                                                  ReorderLevel = pm.ReorderLevel
                                              })
                                              .ToListAsync();

            






            //// Generate image
            //using (var bitmap = new Bitmap(800, 600))
            //using (var graphics = Graphics.FromImage(bitmap))
            //{
            //    graphics.Clear(Color.White);
            //    using (var font = new Font("Arial", 12))
            //    {
            //        int y = 10;
            //        graphics.DrawString("Stock Order Details", new Font("Arial", 16, FontStyle.Bold), Brushes.Black, 10, y);
            //        y += 30;

            //        foreach (var item in medicationsToReorder)
            //        {
            //            graphics.DrawString($"Medication: {item.MedicationName}", font, Brushes.Black, 10, y);
            //            y += 20;
            //            graphics.DrawString($"Form: {item.MedicationForm}", font, Brushes.Black, 10, y);
            //            y += 20;
            //            graphics.DrawString($"Schedule: {item.Schedule}", font, Brushes.Black, 10, y);
            //            y += 20;
            //            graphics.DrawString($"Stock on Hand: {item.StockonHand}", font, Brushes.Black, 10, y);
            //            y += 20;
            //            graphics.DrawString($"Reorder Level: {item.ReorderLevel}", font, Brushes.Black, 10, y);
            //            y += 30;
            //        }

            //        graphics.DrawString($"Notes: {notes}", font, Brushes.Black, 10, y);
            //        y += 30;
            //        graphics.DrawString($"Kind Regards,", font, Brushes.Black, 10, y);
            //        y += 20;
            //        graphics.DrawString($"{PharmacistName} {PharmacistSurname}", font, Brushes.Black, 10, y);
            //    }

            //    // Save image to memory stream
            //    using (var ms = new MemoryStream())
            //    {
            //        bitmap.Save(ms, ImageFormat.Png);
            //        ms.Position = 0;

            //        // Create email message
            //        var emailMessage = new MimeMessage();
            //        emailMessage.From.Add(new MailboxAddress("Day Hospital - Apollo+(Group 9 - 4Year)", PharmacistEmail));
            //        emailMessage.To.Add(new MailboxAddress("Purchasing Manager", "nmostert@nmmu.ac.za"));
            //        emailMessage.Subject = $"Stock Order Request {bitmap}";

            //        var builder = new BodyBuilder();
            //        builder.Attachments.Add("StockOrder.png", ms.ToArray());
            //        emailMessage.Body = builder.ToMessageBody();

            //        // Send email
            //        try
            //        {
            //            using (var client = new MailKit.Net.Smtp.SmtpClient())
            //            {
            //                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //                await client.AuthenticateAsync("nmostert@nmmu.ac.za", "xqqx kiox hcgm xvmr");
            //                await client.SendAsync(emailMessage);
            //                await client.DisconnectAsync(true);
            //            }

            //            return Ok("Email sent successfully");
            //        }
            //        catch (Exception ex)
            //        {
            //            return BadRequest($"Error sending email: {ex.Message}");
            //        }
            //    }


            return RedirectToAction("ViewAllOrders");
        }







        public IActionResult ViewAllPrescriptions()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                       ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

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


                                .Distinct()
                                                             .ToList();



            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);
        }










        public IActionResult AddMedication()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                        ).Count();

            ViewBag.Count = count;


            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

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


            // Fetch medication schedules
            var medSchedules = _dbContext.Medication
                              .Select(m => m.Schedule)
                              .Distinct()
                              .OrderBy(schedule => schedule) // Order by ascending
                              .ToList();



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


                //MedicationForm = df,

                Schedules = medSchedules,
                DosageForms = dfdropdown,
                ActiveIngredientsDropdown = activeingredientslist,



                combinedinfo = combinedData,



            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }





        [HttpPost]
        public IActionResult AddMedicationAction(PharmacyMedicationViewModel model)
        {
            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                       ).Count();

            ViewBag.Count = count;

            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            // Check if the model state is valid



            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;


            if (ModelState.IsValid)
            {
                // Reload the dropdown data if the model is invalid
                PrepareDropDownLists(model);
                return View("AddMedication", model);
            }

            else
            {
                // Create a new Medication entity from the view model
                Medication med = new Medication
                {

                    MedicationName = model.MedicationName,
                    MedicationForm = model.MedicationForm,  // Ensure this has the selected value
                    Schedule = model.Schedule,                // Ensure this has the selected value
                };

                // Add the medication to the database context
                _dbContext.Medication.Add(med);
                _dbContext.SaveChanges();


                // Create a new PharmMedModel and set its properties
                PharmMedModel pharmMed = new PharmMedModel
                {

                    StockonHand = model.StockonHand,
                    ReorderLevel = model.ReorderLevel,
                    MedicationID = med.MedicationID
                };

                // Add the pharmacy medication to the database context
                _dbContext.PharmacyMedication.Add(pharmMed);
                _dbContext.SaveChanges();

                // Set a success message in TempData and redirect to AddMedication action
                TempData["Success"] = "Medication added successfully!";
                return RedirectToAction("ViewAddedMedication");


            }
           

        }



        public void PrepareDropDownLists(PharmacyMedicationViewModel model)
        {
            model.Schedules = _dbContext.Medication
                .Select(m => m.Schedule)
                .Distinct()
                .OrderBy(schedule => schedule)
                .ToList();

            model.DosageForms = _dbContext.Medication
                .Select(m => m.MedicationForm)
                .Distinct()
                .OrderBy(form => form)
                .ToList();

            model.ActiveIngredientsDropdown = _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .OrderBy(name => name)
                .ToList();
        }





        public IActionResult ViewAddedMedication()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;


            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                      ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

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




            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                      ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;
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






        public async Task<IActionResult> ViewSpecificPrescription(int pid)
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;


            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                      ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

            // Retrieve the specific prescription using the provided id
            var prescription = await _dbContext.Prescription.FindAsync(pid);

            if (prescription == null)
            {
                return NotFound();
            }


            //get allergies
            var allAllergies = await (from p in _dbContext.Prescription
                                      join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                      join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                      join pa in _dbContext.PatientAllergy on pi.PatientID equals pa.PatientID
                                      join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID

                                      where p.PrescriptionID == pid
                                      select ai.ActiveIngredientName)
                            .Distinct()
                            .OrderBy(name => name) // Sort in ascending order
                            .ToListAsync();


            //getConditions
            var patientConditions = await (from p in _dbContext.Prescription
                                           join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                           join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                           join pc in _dbContext.PatientConditions on pi.PatientID equals pc.PatientID
                                           join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID

                                           where p.PrescriptionID == pid
                                           select c.ConditionName)
    .Distinct()
    .OrderBy(name => name) // Sort in ascending order
    .ToListAsync();



            //get current medication
            var currentMeds = await (from p in _dbContext.Prescription
                                     join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                     join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                     join pm in _dbContext.patientMedication on pi.PatientID equals pm.PatientID
                                     join m in _dbContext.Medication on pm.MedicationID equals m.MedicationID
                                     where p.PrescriptionID == pid
                                     select m.MedicationName)
                .Distinct()
                .OrderBy(name => name) // Sort in ascending order
                .ToListAsync();


            // Get the specific prescription data along with related patient information
            var allprescribedmeds = await (from p in _dbContext.Prescription
                                           join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                           join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                           join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                           join account in _dbContext.Accounts on p.AccountID equals account.AccountID

                                           join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                           join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                           where p.PrescriptionID == pid // Filter by prescription ID
                                           select m.MedicationName)
                .Distinct()
                .OrderBy(name => name) // Sort in ascending order
                .ToListAsync();





            var allqty = await (from p in _dbContext.Prescription
                                join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                join account in _dbContext.Accounts on p.AccountID equals account.AccountID

                                join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                where p.PrescriptionID == pid // Filter by prescription ID
                                select mi.Quantity)
               .Distinct()
               .OrderBy(name => name) // Sort in ascending order
               .ToListAsync();


            var allinstructions = await (from p in _dbContext.Prescription
                                         join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                         join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                         join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                         join account in _dbContext.Accounts on p.AccountID equals account.AccountID

                                         join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                         join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                         where p.PrescriptionID == pid // Filter by prescription ID
                                         select mi.Quantity)
             .Distinct()
             .OrderBy(name => name) // Sort in ascending order
             .ToListAsync();



            var allmedicationdata = await (from p in _dbContext.Prescription
                                           join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                           join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                           join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                           join account in _dbContext.Accounts on p.AccountID equals account.AccountID
                                           join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                           join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                           where p.PrescriptionID == pid // Filter by prescription ID
                                           orderby m.MedicationName

                                           select new PharmacistViewScriptModel
                                           {

                                               qty = mi.Quantity,
                                               Instructions = mi.Instructions,
                                               medication = m.MedicationName,
                                           })

                       .GroupBy(m => m.medication)
                       .Select(x => x.FirstOrDefault())



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
                                 orderby pv.time


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
                                     Time = pv.time,
                                     qty = mi.Quantity,
                                     Instructions = mi.Instructions,
                                     medication = m.MedicationName,

                                     Date = p.DateGiven,
                                 })

                       .GroupBy(m => m.Time)
                       .Select(x => x.FirstOrDefault())



                       .ToListAsync();








            var viewModel = new PharmacistViewScriptModel
            {
                combinedData = alldata,
                allmedicationinfo = allmedicationdata,
                Allallergy = allAllergies,
                AllConditions = patientConditions,
                AllCurrentMed = currentMeds,
                allpresribedmeds = allprescribedmeds
            }

            ;


            return View(viewModel);
        }












        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewSpecificPrescriptionDispense(PharmacistViewScriptModel model)


        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



          



            ViewBag.UserAccountID = accountID;



            // int dpid = 4047;


            //int pharmid = 1013;

            int dpid = model.PrescriptionID;


            int pharmid = model.pharmacistID;


            var prescription = _dbContext.Prescription.FirstOrDefault(p=>p.PrescriptionID== dpid);

            if (prescription == null) 
            {
                return NotFound("Prescription not found");
            }


            
                DispensedScriptsModel infotoadd = new DispensedScriptsModel
                {

                    PrescriptionID = model.PrescriptionID,
                    AccountID = pharmid,



                };

                _dbContext.DispensedScriptsModel.Add(infotoadd);
            

                prescription.Status = "Dispensed";
                _dbContext.SaveChanges();
            

           

        


            return RedirectToAction("ViewAllPrescriptions", "Pharmacist");

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewSpecificPrescriptionReject(string RejectMessage,PharmacistViewScriptModel model)


        {



            int rpid = 4047;


            int pharmid = 1013;




            var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == rpid);

            if (prescription == null)
            {
                return NotFound("Prescription not found");
            }



            RejectedScriptsModel infotoadd = new RejectedScriptsModel
            {

                PrescriptionID = rpid,
                AccountID = pharmid,
                RejectionReason = RejectMessage




            };

            _dbContext.RejectScriptModel.Add(infotoadd);
            prescription.Status = "Rejected";
            _dbContext.SaveChanges();




            return RedirectToAction("ViewAllPrescriptions", "Pharmacist");

        }




















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
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid
                accountID = 0; // Or handle as required
            }



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;


            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                         ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

            // Query to get the medications that need to be reordered
            var combinedData = (from pm in _dbContext.PharmacyMedication
                                join m in _dbContext.Medication
                                on pm.MedicationID equals m.MedicationID

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



        public ActionResult ConfirmScriptRejectionPage()
        {
            return View();
        }


        public IActionResult IncomingStockPage()
        {
            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                       ).Count();

            ViewBag.Count = count;
            return View();


            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;
        }


        public ActionResult NewCurrentLevelPage()
        {
            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                      ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;
            return View();
        }


        public ActionResult CurrentLevelUpdatedPage()
        {
            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                      ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;
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



            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);



            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            ViewBag.Today = today;
            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                                  ).Count();

            ViewBag.Count = count;


            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

            var combinedData = (from prescription in _dbContext.Prescription


                                join ap in _dbContext.AdmittedPatients
                                on prescription.AdmittedPatientID equals ap.AdmittedPatientID// Assuming AccountID is linked to PatientID

                                join pi in _dbContext.PatientInfo

                                on ap.PatientID equals pi.PatientID




                                join account in _dbContext.Accounts
                                on prescription.AccountID equals account.AccountID

                                join mi in _dbContext.MedicationInstructions on prescription.PrescriptionID equals mi.PrescriptionID
                                join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID


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
                                   .Distinct()
                                .OrderByDescending(item => item.Urgency == "Yes")


                                .ToList();
        





            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);



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

            var count = (from item in _dbContext.Prescription
                         where item.Status == "Prescribed"
                         select item
                                  ).Count();

            ViewBag.Count = count;

            var lowlevelstock = (from pm in _dbContext.PharmacyMedication
                                 join m in _dbContext.Medication
                                 on pm.MedicationID equals m.MedicationID
                                 where pm.StockonHand <= pm.ReorderLevel
                                 select pm).Count();

            ViewBag.LowLevelStock = lowlevelstock;

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
















