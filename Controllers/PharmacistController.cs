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
using Microsoft.IdentityModel.Tokens;

using MimeKit;

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
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;




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

        //[HttpPost]
        //public ActionResult RejectScript(int id, string reason)
        //{
        //    if (string.IsNullOrEmpty(reason))
        //    {
        //        return BadRequest("Reason is required.");
        //    }

        //    // Your logic to save the rejection reason in the database
        //    var prescription = _dbContext.RejectScriptModel.Find(id);
        //    if (prescription != null)
        //    {
        //        prescription.RejectionReason = reason;
        //        prescription.RejectionID = id;
        //        _dbContext.SaveChanges();
        //        return Json(new { success = true });
        //    }

        //    return NotFound();
        //}


        [HttpGet]
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
                                join stock in _dbContext.PharmacyStock
                                on pm.StockonHand equals stock.StockonHand
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

            if ( combinedData.Count == 0 )
            {
                Console.WriteLine("No stock in need of ordering today");
            }

            return View(new PharmacistStockOrderViewModel
            {
                PharmacistStockOrders = combinedData
            });

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOrderPageOrder(PharmacistStockOrderViewModel model)
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


            var medicationsToReorder = await (from pm in _dbContext.PharmacyMedication
                                              join m in _dbContext.Medication
                                              on pm.MedicationID equals m.MedicationID
                                              where pm.StockonHand <= pm.ReorderLevel

                                              select new PharmacistStockOrderViewModel
                                              {
                                                  MedicationName = m.MedicationName,
                                                  MedicationForm = m.MedicationForm,
                                                  Schedule = m.Schedule,
                                                  StockonHand = pm.StockonHand,
                                                  ReorderLevel = pm.ReorderLevel,
                                                  qty=model.qty,
                                                 
                                                  
                                              })
                                              .ToListAsync();

            var stockOrder = medicationsToReorder.Select(m => new PharmMedicationStockOrder
            {
                MedicationName = m.MedicationName,
                MedicationForm = m.MedicationForm,
                Schedule = m.Schedule,

                StockonHand = m.StockonHand,
                ReorderLevel = m.ReorderLevel,
                qtyOrdered = m.qty,
                Status = "Ordered"
            }).ToList();


            _dbContext.PharmacyStock.AddRange(stockOrder);
            _dbContext.SaveChanges();


            //email still not working

            //try
            //{
            //    // Set up SMTP client
            //    using (var smtpClient = new SmtpClient())
            //    {
                    
            //        smtpClient.Port = 587;
            //        smtpClient.Credentials = new NetworkCredential("sam12mensah@gmail.com", "churchposters1!");
            //        smtpClient.EnableSsl = true;

            //        // Create the email message
            //        var mailMessage = new MailMessage
            //        {
            //            From = new MailAddress("sam12mensah@gmail.com"),
            //            Subject = "Stock Order Notification",
            //            IsBodyHtml = true
            //        };

            //        // Add recipient(s)
            //        mailMessage.To.Add("s223130680@mandela.ac.za");  // Replace with your recipient's email address

            //        // Build the email body with stock order details
            //        var emailBody = "<h3>Stock Order Details</h3>";
            //        emailBody += "<table border='1'><tr><th>Medication Name</th><th>Dosage Form</th><th>Schedule</th><th>Stock on Hand</th><th>Reorder Level</th><th>Quantity Ordered</th></tr>";

            //        foreach (var order in stockOrder)
            //        {
            //            emailBody += $"<tr><td>{order.MedicationName}</td><td>{order.MedicationForm}</td><td>{order.Schedule}</td><td>{order.StockonHand}</td><td>{order.ReorderLevel}</td><td>{order.qtyOrdered}</td></tr>";
            //        }

            //        emailBody += "</table>";
            //        emailBody += "<p>This is an automated email. Please do not reply.</p>";

            //        // Set the email body
            //        mailMessage.Body = emailBody;

            //        // Send email asynchronously
            //        await smtpClient.SendMailAsync(mailMessage);
            //    }
            //    TempData["PopupMessage"] = "Medication Ordered and Email Sent";
            //}
            //catch (Exception ex)
            //{
            //    // Handle any exceptions related to email sending
            //    // For example, log the error (you can implement a logger in your application)
            //    Console.WriteLine($"Error sending email: {ex.Message}");
            //    TempData["PopupMessage"] = "An error occurred while sending the email.";

            //}










            return RedirectToAction ("ViewAllOrders");
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
        public IActionResult AddMedicationAction(int medicationid,PharmacyMedicationViewModel model)
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
               

                PrepareDropDownLists(model);

                Models.MedicationActiveIngredient activeandstrengthtoadd = new Models.MedicationActiveIngredient
                {

                    MedicationID = model.MedicationID,
                    ActiveingredientID = model.aiID,
                    ActiveIngredientStrength = model.aiStrength


                };

                _dbContext.MedicationActiveIngredient.Add(activeandstrengthtoadd);
                _dbContext.SaveChanges();

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




               





                //}

                //// Save changes to the database


                //return Json(new { success = true, message = "Medication added successfully." });




                // Set a success message in TempData and redirect to AddMedication action
                TempData["Success"] = "Medication added successfully!";
                return RedirectToAction("ViewAddedMedication");


            }
           

        }


        public class Ingredient
        {
            public string Name { get; set; }

            public int ID { get; set; }
            public int Strength { get; set; }
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





        [HttpGet]
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


            ViewBag.ScriptID = pid;


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



            //var medinteractionalert = (from p in _dbContext.Prescription
            //                           join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
            //                           join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
            //                           join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
            //                           join pm in _dbContext.patientMedication on pi.PatientID equals pm.PatientID
            //                           join account in _dbContext.Accounts on p.AccountID equals account.AccountID
            //                           join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
            //                           join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
            //                           join cm in _dbContext.patientMedication on m.MedicationID equals cm.MedicationID

            //                           join medinteract in _dbContext.Medication on cm.MedicationID equals medinteract.MedicationID
            //                           join medinteract2 in _dbContext.patientMedication on medinteract.MedicationID equals medinteract2.MedicationID

            //                           where p.PrescriptionID == pid  // Make sure 'pid' is being passed correctly


            //               && (
            //                   (
            //                   m.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Neo-Mercazole").MedicationID
            //                   &&
            //                   cm.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Cardura 8mg").MedicationID)
            //                   ||

            //                   ((m.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Cardura 8mg").MedicationID
            //                   &&
            //                   cm.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Neo-Mercazole").MedicationID)
            //                  )

            //               )

            //                           select new PharmacistViewScriptModel
            //                           {
            //                               PrescriptionID = p.PrescriptionID,
            //                               medicationname = m.MedicationName,
            //                               medicationid = cm.MedicationID,
            //                               // Add other fields you need from the joins
            //                               // You can also add any medication interaction checks you need here
            //                           })





            //                 .Distinct()
            //                 .OrderBy(name => name.medicationname) // Sort by MedicationName in ascending order
            //                 .ToList();




            var medinteractionalert = (from p in _dbContext.Prescription
                                       join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                       join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                       join pm in _dbContext.patientMedication on pi.PatientID equals pm.PatientID
                                       join account in _dbContext.Accounts on p.AccountID equals account.AccountID
                                       join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                       join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID
                                       join cm in _dbContext.patientMedication on pi.PatientID equals cm.PatientID
                                       join medinteract in _dbContext.Medication on cm.MedicationID equals medinteract.MedicationID
                                       // Alias for MedicationName in patientMedication
                                       let cmMedicationName = _dbContext.MedicationInstructions
                                                                       .Where(med => med.MedicationID == cm.MedicationID && med.PrescriptionID == p.PrescriptionID && med.MedicationID== m.MedicationID)
                                                                       .Select(med => m.MedicationName)
                                                                       .FirstOrDefault()  // Get MedicationName of cm
                                       where p.PrescriptionID == pid  // Ensure 'pid' is being passed correctly
                                             && (
                                                  // Checking for Neo-Mercazole and Cardura 8mg interaction
                                                  (m.MedicationName == "Neo-Mercazole" && cmMedicationName == "Cardura 8mg") ||
                                                  (m.MedicationName == "Cardura 8mg" && cmMedicationName == "Neo-Mercazole")



                                                  &&

                                                   (m.MedicationName == "Cardura 8mg" && cmMedicationName == "Neo-Mercazole") ||
                                                  (m.MedicationName == "Cardura 8mg" && cmMedicationName == "Cardura 8mg")

                                             )


                                       select new PharmacistViewScriptModel
                                       {
                                           PrescriptionID = p.PrescriptionID,
                                           PatientID = pi.PatientID,
                                           medicationname = m.MedicationName,
                                          
                                       }).ToList();




            //&& (
            //    (m.MedicationName == "Neo-Mercazole" && cmMedicationName == "Cardura 8mg") ||
            //    (m.MedicationName == "Cardura 8mg" && cmMedicationName == "Neo-Mercazole")
            //)

            //&&
            // (m.MedicationID == _dbContext.Medication.FirstOrDefault(mia=>mia.MedicationName== "Neo-Mercazole").MedicationID

            // && 

            // cm.MedicationID ==_dbContext.Medication.FirstOrDefault(mia=>mia.MedicationName== "Cardura 8mg") .MedicationID)

            // ||

            //  (m.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Cardura 8mg").MedicationID

            // &&

            // cm.MedicationID == _dbContext.Medication.FirstOrDefault(mia => mia.MedicationName == "Neo-Mercazole").MedicationID) 





            //select new PharmacistViewScriptModel
            //{
            //    PrescriptionID = p.PrescriptionID,
            //    medicationname = m.MedicationName,
            //    medicationid = cm.MedicationID
            //})
            // .Distinct()  // Remove duplicates
            // .OrderBy(name => name.medicationname)  // Sort by MedicationName
            // .ToList();




            // Pass data to the view


            if (medinteractionalert.Any())
            {
                // Set a message for the alert if any interactions were found
                ViewBag.MedicationInteractionAlert = "Warning: Medication interactions detected! Carbimazole(Neo-Mercazole) interacts with Doxazosin(Cardura)";
            }





            var viewModel = new PharmacistViewScriptModel
            {
                combinedData = alldata,
                allmedicationinfo = allmedicationdata,
                Allallergy = allAllergies,
                AllConditions = patientConditions,
                AllCurrentMed = currentMeds,
                allpresribedmeds = allprescribedmeds,
                medicationinteractionsalerts= medinteractionalert,
            }

            ;


            return View(viewModel);
        }












        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>ViewSpecificPrescription(int pid, int pharmid, int pharmid2, PharmacistViewScriptModel model)


        {
           

            

            




           


           

            model.PrescriptionID = pid;




            //int dpid = 4047;


            // pharmid = 1013;

            //int mybulenid = 1002;

            //int dpid = model.PrescriptionID;


            //int pharmid = model.pharmacistID;




            var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == pid);

            if (prescription == null)
            {
                return NotFound("Prescription not found");
            }

            //var medication = _dbContext.PharmacyMedication.FirstOrDefault(m => m.MedicationID == 1002);

            //if (medication == null)
            //{
            //    return NotFound("Medication not found in stock");
            //}

            //if (medication.StockonHand >= 20)
            //{
            //    medication.StockonHand -= 20; // Subtract 20 from the stock
            //}
            //else
            //{
            //    // Handle the case where there is insufficient stock
            //    return BadRequest("Insufficient stock to dispense the medication");
            //}






            DispensedScriptsModel infotoadd = new DispensedScriptsModel
                {

                    PrescriptionID = pid,
                    AccountID = pharmid2,



                };

                _dbContext.DispensedScriptsModel.Add(infotoadd);
            

                prescription.Status = "Dispensed";

                _dbContext.SaveChanges();


            var allmedicationdata = await (from p in _dbContext.Prescription
                                           join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                           join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                           join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                           join account in _dbContext.Accounts on p.AccountID equals account.AccountID
                                           join mi in _dbContext.MedicationInstructions on p.PrescriptionID equals mi.PrescriptionID
                                           join m in _dbContext.Medication on mi.MedicationID equals m.MedicationID

                                           where p.PrescriptionID == pid /// Filter by prescription ID
                                           orderby m.MedicationName

                                           select new PharmacistViewScriptModel
                                           {

                                               qty = mi.Quantity,
                                               Instructions = mi.Instructions,
                                               medication = m.MedicationName,
                                               medicationid = m.MedicationID,
                                           })

                        .GroupBy(m => m.medication)
                        .Select(x => x.FirstOrDefault())



                        .ToListAsync();


            var medicationIds = allmedicationdata.Select(m => m.medicationid).ToList();

            // Fetch the stock for the medications in a single query
            var stockList = await _dbContext.PharmacyMedication
                                             .Where(pm => medicationIds.Contains(pm.MedicationID))
                                             .ToListAsync();

            // Loop through the medications and adjust the stock
            foreach (var meds in allmedicationdata)
            {
                // Find the stock record matching the current medication
                var stock = stockList.FirstOrDefault(pm => pm.MedicationID == meds.medicationid);

                if (stock != null)
                {
                    // Subtract the prescribed quantity from the stock
                    stock.StockonHand -= meds.qty;

                    // Ensure that the stock doesn't go negative
                    if (stock.StockonHand < 0)
                    {
                        stock.StockonHand = 0; // Optional: Handle as you prefer (e.g., throw an exception, log a warning)
                    }

                    // Update the stock record in the database
                    _dbContext.PharmacyMedication.Update(stock);
                    await _dbContext.SaveChangesAsync();

                }
            }




            return RedirectToAction("ViewAllActivePrescriptionsPage", "Pharmacist");

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ViewSpecificPrescriptionReject(int pid, int pharmid, int pharmid2, string rejectionMessage,PharmacistViewScriptModel model)


        {
            model.PrescriptionID= pid;
         
         




            var prescription = _dbContext.Prescription.Find(pid);

            if (prescription == null)
            {
                return NotFound("Prescription not found");
            }


        //int rpid = 4047;


        //    int pharmid = 1013;

            


            //var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == rpid);

            //if (prescription == null)
            //{
            //    return NotFound("Prescription not found");
            //}



            RejectedScriptsModel infotoadd = new RejectedScriptsModel
            {

                PrescriptionID = pid,
                AccountID = pharmid2,
                RejectionReason= rejectionMessage,





            };

            _dbContext.RejectScriptModel.Add(infotoadd);
            prescription.Status = "Rejected";
            _dbContext.SaveChanges();




            return RedirectToAction("ViewAllActivePrescriptionsPage", "Pharmacist");

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


            var stocktoshow= _dbContext.PharmacyStock.ToList();

            var viewModel = new PharmacistStockOrderViewModel
            {
                StockOrder = stocktoshow
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
















