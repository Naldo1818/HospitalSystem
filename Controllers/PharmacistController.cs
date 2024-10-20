﻿using DEMO.Data;
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
using Newtonsoft.Json;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using DEMO.Models.PharmacistModels;
using MailKit.Net.Smtp;
using System.Linq;
using System.Net;
using System.Threading.Tasks.Dataflow;
using System.Security.Principal;



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


        public async Task<IActionResult> StockOrderPage()
        {
            var stocks = await _dbContext.PharmacyMedication
                  .Where(m => m.StockonHand <= m.ReorderLevel)
                  .ToListAsync();

            return View(stocks);

        }

      




        public IActionResult ViewAllPrescriptions()
        {
            //var accountIDString = HttpContext.Session.GetString("UserAccountId");
            //if (!int.TryParse(accountIDString, out int accountID))
            //{
            //    // Handle the case where accountID is not available or is invalid
            //    accountID = 0; // Or handle as required
            //}

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
                                   MedicationName= m.MedicationName,
                                   MedicationForm= m.MedicationForm,
                                   Schedule=m.Schedule,
                                   StockonHand=pm.StockonHand,
                                   ReorderLevel=pm.ReorderLevel,
                                  
                                   




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


                MedicationForm=df,
                
                Schedules=medSchedules,
                DosageForms=dfdropdown,
                ActiveIngredientsDropdown=activeingredientslist,
                


                combinedinfo =combinedData,

              
                









                

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

            model.ActiveIngredientsDropdown=activeingredientslist;
            model.Schedules=schedules;
            model.DosageForms=dosageforms;
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

        //[HttpGet]
        //public async Task<IActionResult> ViewSpecificPrescription(int pid)
        //{
        //  var prescription  =   await _dbContext.Prescription(pid);


        //    if (prescription == null)
        //    {
        //        return NotFound();
        //    }


        //    // Retrieve all conditions, allergies, and current medications
        //    var allConditions = _dbContext.Condition
        //        .Select(m => m.ConditionName)
        //        .Distinct()
        //        .OrderBy(name => name)
        //        .ToList();

        //    var allAllergies = _dbContext.Activeingredient
        //        .Select(m => m.ActiveIngredientName)
        //        .Distinct()
        //        .OrderBy(name => name)
        //        .ToList();

        //    var currentMeds = _dbContext.Medication
        //        .Select(m => m.MedicationForm)
               
        //        .Distinct()
        //        .OrderBy(name => name)
                
        //        .ToList();


        //    // Get the specific prescription using the provided id
        //    var alldata = (from p in _dbContext.Prescription
        //                   join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
        //                   join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
        //                   join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
        //                   join account in _dbContext.Accounts on p.AccountID equals account.AccountID
        //                   where p.PrescriptionID == pid // Filter by prescription ID

        //                   select new PharmacistViewScriptModel
        //                   {
                              
        //                       Urgency = p.Urgency,
        //                       PrescriptionID = p.PrescriptionID,
        //                       AdmittedPatientID = p.AdmittedPatientID,
        //                       patientname = pi.Name,
        //                       patientsurname = pi.Surname,
        //                       Status = p.Status,
        //                       Height = ap.Height,
        //                       Weight = ap.Weight,
        //                       SystolicBloodPressure = pv.SystolicBloodPressure,
        //                       DiastolicBloodPressure = pv.DiastolicBloodPressure,
        //                       HeartRate = pv.HeartRate,
        //                       BloodOxygen = pv.BloodOxygen,
        //                       Respiration = pv.Respiration,
        //                       BloodGlucoseLevel = pv.BloodGlucoseLevel,
        //                       Temperature = pv.Temperature,
        //                       SurgeonName = account.Name,
        //                       SurgeonSurname = account.Surname,
        //                   })

        //                   .ToList();


         




        //    var viewModel = new PharmacistViewScriptModel
        //    {
        //        combinedData = alldata,
        //        Allallergy = allAllergies,
        //        AllConditions = allConditions,
        //        AllCurrentMed = currentMeds,
        //    };

        //    return View(viewModel);
        //}


        [HttpGet]
        public async Task<IActionResult> ViewSpecificPrescription(int pid)
        {
            // Retrieve the specific prescription using the provided id
            var prescription = await _dbContext.Prescription.FindAsync(pid);

            if (prescription == null)
            {
                return NotFound();
            }

            var admittedpatientid= prescription.AdmittedPatientID;

    





            var allConditions = await _dbContext.Condition
                .Select(m => m.ConditionName)
                .Distinct()
                
                .OrderBy(name => name)
                .ToListAsync();

            var allAllergies = await _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            var currentMeds = await _dbContext.Medication
                .Select(m => m.MedicationForm)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();

            // Get the specific prescription data along with related patient information
            var alldata = await (from p in _dbContext.Prescription
                                 join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                 join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                 join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID
                                 join account in _dbContext.Accounts on p.AccountID equals account.AccountID
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
                                 })
                                  .ToListAsync();

            // Create the view model with combined data and additional information
            var viewModel = new PharmacistViewScriptModel
            {
                combinedData = alldata, // Ensure property names match your model's definition
                Allallergy = allAllergies,
                AllConditions = allConditions,
                AllCurrentMed = currentMeds,
            };

            return View(viewModel);
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
                                .OrderByDescending(item => item.Urgency=="Yes")
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
                                    Patient=p.Name + " " + p.Surname,
                                    Surgeon=pharmacist.Name + " " + pharmacist.Surname,

                                    
                                    TreatmentCode = tc.TreatmentCode,
                                    TreatmentName = tc.TreatmentName
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













    

