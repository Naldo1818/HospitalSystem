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
using Newtonsoft.Json;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Diagnostics;
using DEMO.Models.PharmacistModels;
using MailKit.Net.Smtp;
using System.Linq;
using System.Net;



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
            var stocks = await _dbContext.DayHospitalPharmacyMedication
                  .Where(m => m.StockonHand <= m.ReorderLevel)
                  .ToListAsync();

            return View(stocks);

        }



        //Auto order function

        //public void AutoOrderProducts(PharmacyMedicationModel product)
        //{
        //    var productsToReorder = _dbContext.DayHospitalPharmacyMedication
        //       .Where(p => p.StockonHand <= p.ReorderLevel)
        //       .ToList();

        //    foreach (var product in productsToReorder)
        //    {
        //        PlaceOrder(product);
        //    }
        //}

        //public void PlaceOrder(PharmacyMedicationModel product)
        //{
        //    var order = new OrderStockModel
        //    {

        //        OrderDate = DateTime.Now


        //        //new OrderItem
        //        //{
        //        //    ProductId = product.Id,
        //        //     // Assuming you have a price property
        //        //}

        //    };

        //    // Save the order to the database
        //    _dbContext.StockOrderedTable.Add(order);
        //    _dbContext.SaveChanges();
        //}

        public IActionResult ViewAllPrescriptions()
        {
            //var accountIDString = HttpContext.Session.GetString("UserAccountId");
            //if (!int.TryParse(accountIDString, out int accountID))
            //{
            //    // Handle the case where accountID is not available or is invalid
            //    accountID = 0; // Or handle as required
            //}

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


                                select new ViewActivePrescriptionsModel


                                {
                                    // Prescription fields
                                    PrescriptionID = prescription.PrescriptionID,
                                    SurgeonName = account.Name,
                                    SurgeonSurname = account.Surname,
                                    DateGiven = prescription.DateGiven,
                                    Urgency = prescription.Urgency,
                                    Take = prescription.Take,
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
            // Fetch medication names


            // Fetch medication forms
            var medicationForms = _dbContext.Medication
                                             .Select(m => m.MedicationForm)
                                             .Distinct()
                                             .ToList();

            // Fetch medication schedules
            var medSchedules = _dbContext.Medication
                                         .Select(m => m.Schedule)
                                         .Distinct()
                                         .ToList();


            //Fetch Active Ingredients
            var actives = _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .Distinct()
                .ToList();

           

            var df = _dbContext.DayHospitalPharmacyMedication
               .Select(m => m.DosageForm)
               .Distinct()
               .ToString();

            var schedule = _dbContext.DayHospitalPharmacyMedication
                .Select(m => m.Schedule)
                .Distinct()
                .ToString();


            var stockonhand = _dbContext.DayHospitalPharmacyMedication
                .Select(m => m.StockonHand)
                .Distinct()
                .ToString();


            var activeingredientslist=_dbContext.Activeingredient
                .Select(m=>m.ActiveIngredientName)
                .Distinct()
                .ToList();



            //al for table display
            var names = _dbContext.DayHospitalPharmacyMedication
             .Select(m => m.MedicationName)
             .Distinct()
             .ToString();


           var combineddata=_dbContext.DayHospitalPharmacyMedication
                .Select(m=>m)
                .ToList();




            // Create a ViewModel to hold the data
            var viewModel = new PharmacyMedicationModel
            {


                 ActiveIngredientsDropDown = activeingredientslist,
                DosageForm=df,
                PharmMedDF = medicationForms,
                PharmMedSchedule = medSchedules,
                MedicationName=names,
                combined=combineddata,
               

                


                //testMeds=new PharmacyMedicationModel()

            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }




        //post medication to database
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddMedication(PharmacyMedicationModel model, string IngredientsString, string StrengthsString)
        //{


        //    if (ModelState.IsValid)
        //    {



        //        var detailstoadd = new PharmacyMedicationModel
        //        {
        //            PharmacyMedicationID = model.PharmacyMedicationID,
        //            MedicationName = model.MedicationName,
        //            DosageForm = model.DosageForm,
        //            Schedule = model.Schedule,
        //            StockonHand = model.StockonHand,
        //            ReorderLevel = model.ReorderLevel,
        //            PharmMedDF=model.PharmMedDF,
        //            PharmMedSchedule=model.PharmMedSchedule,

        //        };

        //        _dbContext.DayHospitalPharmacyMedication.Add(detailstoadd);
        //        _dbContext.SaveChanges();




        //        //return RedirectToAction("AddMedication", "Pharmacist");
        //        return RedirectToAction("AddMedication");



        //    }


        //    model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).Distinct().ToList();
        //    model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).Distinct().ToList();
        //    model.ActiveIngredientsDropDown=_dbContext.Activeingredient.Select(m=>m.ActiveIngredientName).Distinct().ToList();  
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddMedication(PharmacyMedicationModel model, string IngredientsAndStrengthsString)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Split the combined string into individual pairs
        //        var ingredientStrengthPairs = IngredientsAndStrengthsString.Split(';')
        //            .Select(pair => pair.Trim())
        //            .Where(pair => !string.IsNullOrWhiteSpace(pair))
        //            .ToList();

        //        // Create a new PharmacyMedicationModel instance
        //        var detailstoadd = new PharmacyMedicationModel
        //        {
        //            PharmacyMedicationID = model.PharmacyMedicationID,
        //            MedicationName = model.MedicationName,
        //            DosageForm = model.DosageForm,
        //            Schedule = model.Schedule,
        //            StockonHand = model.StockonHand,
        //            ReorderLevel = model.ReorderLevel,
        //            PharmMedDF = model.PharmMedDF,
        //            PharmMedSchedule = model.PharmMedSchedule,
        //        };

        //        // Add the medication to the database
        //        _dbContext.DayHospitalPharmacyMedication.Add(detailstoadd);
        //        _dbContext.SaveChanges();

        //        // Now save the active ingredients and strengths
        //        foreach (var pair in ingredientStrengthPairs)
        //        {
        //            var parts = pair.Split(',')
        //                .Select(part => part.Trim())
        //                .ToList();

        //            if (parts.Count == 2)
        //            {
        //                var ingredient = parts[0].Trim();
        //                var strength = parts[1].Trim();

        //                if (!string.IsNullOrWhiteSpace(ingredient) && !string.IsNullOrWhiteSpace(strength))
        //                {
        //                    // Combine ingredient and strength into one variable
        //                    var combined = $"{ingredient}:{strength}";

        //                    // Create a new entity for the active ingredient
        //                    var activeIngredient = new PharmacyMedicationModel
        //                    {
        //                        IngredientandStrength = combined // Assuming you have a property for this
        //                    };

        //                    _dbContext.DayHospitalPharmacyMedication.Add(activeIngredient);
        //                }
        //            }


        //            // Save changes for the active ingredients
        //            _dbContext.SaveChanges();

        //            // Redirect to the AddMedication page or another page
        //            return RedirectToAction("AddMedication");
        //        }

        //        // If the model state is invalid, repopulate the model data
        //        model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).Distinct().ToList();
        //        model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).Distinct().ToList();
        //        model.ActiveIngredientsDropDown = _dbContext.Activeingredient.Select(m => m.ActiveIngredientName).Distinct().ToList();
        //        return View(model);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMedication(PharmacyMedicationModel model, string IngredientsAndStrengthsString)
        {
            if (ModelState.IsValid)
            {
              

                // Create a new PharmacyMedicationModel instance
                var detailstoadd = new PharmacyMedicationModel
                {
                    PharmacyMedicationID = model.PharmacyMedicationID,
                    MedicationName = model.MedicationName,
                    DosageForm = model.DosageForm,
                    Schedule = model.Schedule,
                    StockonHand = model.StockonHand,
                    ReorderLevel = model.ReorderLevel,
                    PharmMedDF = model.PharmMedDF,
                    PharmMedSchedule = model.PharmMedSchedule,
                    ActiveIngredientsDropDown = model.ActiveIngredientsDropDown,
                    
                };

                // Add the medication to the database
                _dbContext.DayHospitalPharmacyMedication.Add(detailstoadd);
                _dbContext.SaveChanges(); // Save here to get the ID for the next inserts

                // Split the combined string into individual pairs
                var ingredientStrengthPairs = IngredientsAndStrengthsString.Split(';')
      .Select(pair => pair.Trim())
      .Where(pair => !string.IsNullOrWhiteSpace(pair))
      .ToList();


                // Now save the active ingredients and strengths
                foreach (var pair in ingredientStrengthPairs)
                {
                    var parts = pair.Split(',')
                       .Select(part => part.Trim())
                       .ToList();

                    if (parts.Count == 2)
                    {
                        var ingredient = parts[0];
                        var strength = parts[1];

                        if (!string.IsNullOrWhiteSpace(ingredient) && !string.IsNullOrWhiteSpace(strength))
                        {
                            var combined = $"{ingredient}:{strength}";

                            var activeIngredient = new PharmacyMedicationModel
                            {
                                // Link to the medication
                                PharmacyMedicationID = detailstoadd.PharmacyMedicationID,
                                IngredientandStrength = combined
                            };

                            _dbContext.DayHospitalPharmacyMedication.Add(activeIngredient);
                        }
                    }
                }
                
                // Save changes for the active ingredients
                _dbContext.SaveChanges();

                // Redirect to the AddMedication page or another page
                return RedirectToAction("AddMedication");
            }

            // If the model state is invalid, repopulate the model data
            
    model.MedicationName = "";
            model.StockonHand = 0; // Assuming StockOnHand is an integer
            model.ReorderLevel = 0; // Assuming ReorderLevel is an integer

            model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).Distinct().ToList();
            model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).Distinct().ToList();
            model.ActiveIngredientsDropDown = _dbContext.Activeingredient.Select(m => m.ActiveIngredientName).Distinct().ToList();
            return View(model);
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


      

        public IActionResult ViewSpecificPrescription(int id)
        {

            var patient = _dbContext.Prescription.Find(id);

            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);

            //         var ScriptAndVitals = (from p in _dbContext.Prescription
            //                                    join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
            //                                    join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
            //                                    join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID

            //                                    where p.PrescriptionID == prescriptionid


            //                                    select new PharmacistViewScriptModel
            //                                    {
            //                                        Take = p.Take,
            //                                        Urgency = p.Urgency,
            //                                        PrescriptionID = p.PrescriptionID,
            //                                        PatientID = pi.PatientID,
            //                                        patientname = pi.Name,
            //                                        patientsurname = pi.Surname,
            //                                        Status = p.Status,
            //                                        Height=pv.Height,
            //                                        Weight=pv.Weight,
            //                                        SystolicBloodPressure=pv.SystolicBloodPressure,  
            //                                        DiastolicBloodPressure=pv.DiastolicBloodPressure,
            //                                        HeartRate=pv.HeartRate,
            //                                        BloodOxygen=pv.BloodOxygen,
            //                                        Respiration=pv.Respiration,
            //                                        BloodGlucoseLevel=pv.BloodGlucoseLevel,
            //                                        Temperature=pv.Temperature,



            //                                    })

            //.ToList();




            //         var viewModel = new PharmacistViewScriptModel
            //         {

            //             combinedData = ScriptAndVitals,



            //         };


            //         return View(viewModel);



        }







        //         var patientVitals = (from pv in _dbContext.PatientVitals
        //                              join ap in _dbContext.AdmittedPatients
        //                              on pv.PatientID equals ap.PatientID
        //                              where pv.PatientID == patientid

        //                              select new PharmacistViewScriptModel
        //                              {
        //                                  Date = ap.Date,
        //                                  Time = pv.time,
        //                                  Height = pv.Height,
        //                                  Weight = pv.Weight,
        //                                  SystolicBloodPressure = pv.SystolicBloodPressure,
        //                                  DiastolicBloodPressure = pv.DiastolicBloodPressure,
        //                                  HeartRate = pv.HeartRate,
        //                                  BloodOxygen = pv.BloodOxygen,
        //                                  Respiration = pv.Respiration,
        //                                  BloodGlucoseLevel = pv.BloodGlucoseLevel,
        //                                  Temperature = pv.Temperature


        //                              }).OrderBy(ap => ap.Date).ToList();


        //         var allergy = (from pa in _dbContext.PatientAllergy
        //                        join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
        //                        join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
        //                        where pa.PatientID == patientid
        //                        select new PharmacistViewScriptModel
        //                        {
        //                            patientname = p.Name,
        //                            patientsurname = p.Surname,
        //                            ActiveIngredientName = ai.ActiveIngredientName
        //                        })
        //       .OrderBy(ai => ai.ActiveIngredientName)
        //       .ToList();




        //         var conditions = (from pc in _dbContext.PatientConditions
        //                           join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
        //                           join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID
        //                           where pc.PatientID == patientid

        //                           select new PharmacistViewScriptModel
        //                           {
        //                               patientname = pt.Name,
        //                               patientsurname = pt.Surname,
        //                               Condition = c.ConditionName // Ensure this property exists in your view model
        //                           }).OrderBy(c => c.Condition).ToList();


        //         var currentMed = (from pm in _dbContext.patientMedication
        //                           join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
        //                           join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
        //                           where pm.PatientID == patientid
        //                           select new PharmacistViewScriptModel
        //                           {
        //                               patientname = pi.Name,
        //                               patientsurname = pi.Surname,
        //                               patientMedication = cm.MedicationName // Ensure this property exists in your view model
        //                           }).OrderBy(cm => cm.patientMedication).ToList();

        //         var viewModel = new PharmacistViewScriptModel
        //         {
        //             Allvitals = patientVitals,
        //             Allallergy = allergy,
        //             AllConditions = conditions,
        //             AllCurrentMed = currentMed,
        //             PrescrptionDetails= prescriptiondetails,


        //         };




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
                                    Take = prescription.Take,
                                    Status = prescription.Status,
                                    AdmittedPatientID = prescription.AdmittedPatientID,

                                    // Medication fields


                                    // MedicationInstructions fields


                                    // PatientInfo fields
                                    Name = pi.Name,
                                    Surname = pi.Surname,



                                })
                                .OrderByDescending(item => item.Urgency=="Yes")
                                .ToList();

            var sortedData = combinedData.OrderByDescending(item => item.Urgency == "Yes").ToList();

            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);


           
        }



      
































    }
}













    

