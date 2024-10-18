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




        public IActionResult ViewAllPrescriptions()
        {
            //var accountIDString = HttpContext.Session.GetString("UserAccountId");
            //if (!int.TryParse(accountIDString, out int accountID))
            //{
            //    // Handle the case where accountID is not available or is invalid
            //    accountID = 0; // Or handle as required
            //}

            var combinedData = (from prescription in _dbContext.Prescription
                                join medicationInstruction in _dbContext.MedicationInstructions
                                on prescription.PrescriptionID equals medicationInstruction.PrescriptionID
                                join medication in _dbContext.Medication
                                on medicationInstruction.MedicationID equals medication.MedicationID
                                join patient in _dbContext.PatientInfo
                                on prescription.AccountID equals patient.PatientID // Assuming AccountID is linked to PatientID
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
                                    Name = patient.Name,
                                    Surname = patient.Surname,



                                }).ToList();



            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);
        }


        //adding pharmacy medication

        // GET: AddMedication
        
        public IActionResult AddMedication()
        {
            // Fetch medication names
            

            // Fetch medication forms
            var medicationForms = _dbContext.Medication
                                             .Select(m => m.MedicationForm)
                                             .ToList();

            // Fetch medication schedules
            var medSchedules = _dbContext.Medication
                                         .Select(m => m.Schedule)
                                         .ToList();


            //Fetch Active Ingredients
            var actives= _dbContext.Activeingredient
                .Select(m => m.ActiveIngredientName)
                .ToList();
            // Create a ViewModel to hold the data
            var viewModel = new PharmacyMedicationModel
            {



                PharmMedDF = medicationForms,
                PharmMedSchedule = medSchedules,
                
                //testMeds=new PharmacyMedicationModel()

            };

            // Pass the ViewModel to the view
            return View(viewModel);
        }




        //post medication to database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddMedication(PharmacyMedicationModel model)
        {
            
                
            if (ModelState.IsValid)
            {



                var detailstoadd = new PharmacyMedicationModel
                {
                    
                    MedicationName = model.MedicationName,
                    DosageForm = model.DosageForm,
                    Schedule = model.Schedule,
                    StockonHand = model.StockonHand,
                    ReorderLevel = model.ReorderLevel,
                    PharmMedDF=model.PharmMedDF,
                    PharmMedSchedule=model.PharmMedSchedule,
                };

                _dbContext.DayHospitalPharmacyMedication.Add(detailstoadd);
                _dbContext.SaveChanges();
                //return RedirectToAction("AddMedication", "Pharmacist");
                return RedirectToAction("AddMedication","Pharmacist");  // Redirect to the product list

            }

            model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).ToList();
            model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).ToList();

            return View(model);
        }


        //public async Task<IActionResult> AddMedication([Bind("MedicationName","DosageForm","Schedule","StockonHand","ReorderLevel")]


        //    PharmacyMedicationModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Create a new medication entity from the model

        //        var medicationEntity = new PharmacyMedicationModel
        //        {

        //            MedicationName = model.MedicationName,
        //            DosageForm = model.DosageForm,
        //            Schedule = model.Schedule,
        //            StockonHand = model.StockonHand,
        //            ReorderLevel = model.ReorderLevel,
        //            //IngredientandStrength = model.IngredientandStrength,
        //            //IngredientsplusStrength = model.IngredientsplusStrength,

        //        };

        //        _dbContext.DayHospitalPharmacyMedication.Add(medicationEntity);


        //        await _dbContext.SaveChangesAsync();
        //        return RedirectToAction("AddMedication"); // Adjust as needed
        //    }

        //    // If validation fails, repopulate dropdowns and return to view with current model
        //    model.PharmacyMedications = _dbContext.Medication.Select(m => m.MedicationName).ToList();
        //    model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).ToList();
        //    model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).ToList();

        //    return View(model);
        //}











        //public async Task<IActionResult> AddMedication(/*[Bind("PharmacyMedicationID,MedicationName,DosageForm,Schedule,StockonHand,ReorderLevel")]*/ AddMedicationViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Create a new medication entity from the model
        //        //var medication = new AddMedicationViewModel
        //        //{
        //        //    testMeds=model.testMeds,

        //        //};


        //        var medicationEntity = new PharmacyMedicationModel
        //        {
        //            MedicationName = model.testMeds.MedicationName,
        //            DosageForm = model.testMeds.DosageForm,
        //            Schedule = model.testMeds.Schedule,
        //            StockonHand = model.testMeds.StockonHand,
        //            ReorderLevel = model.testMeds.ReorderLevel
        //        };

        //        _dbContext.DayHospitalPharmacyMedication.Add(medicationEntity);

        //        //_dbContext.DayHospitalPharmacyMedication.Add(model.testMeds);
        //        await _dbContext.SaveChangesAsync();
        //        return RedirectToAction("AddMedication"); // Adjust as needed


        //    }

        //    // If validation fails, repopulate dropdowns and return to view with current model
        //    model.PharmacyMedications = _dbContext.Medication.Select(m => m.MedicationName).ToList();
        //    model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).ToList();
        //    model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).ToList();
        //    return View(model);
        //}

        //MedicationName=model.testMeds.MedicationName,
        //DosageForm=model.testMeds.DosageForm,
        //Schedule=model.testMeds.Schedule,
        //StockonHand=model.testMeds.StockonHand,
        //ReorderLevel=model.testMeds.ReorderLevel,
        //IngredientandStrength=model.testMeds.IngredientandStrength,
        //IngredientsplusStrength=model.testMeds.IngredientsplusStrength,
        // Add additional properties as needed




        //public async Task<IActionResult> AddMedication([Bind

        //    ("PharmacyMedicationID,MedicationName,DosageForm,Schedule,StockonHand,ReorderLevel")]
        //PharmacyMedicationModel pharmacymedication)


        //{
        //    if (ModelState.IsValid)
        //    {
        //        _dbContext.Add(pharmacymedication);
        //        await _dbContext.SaveChangesAsync();
        //        return RedirectToAction("MedicationAdded", "Pharmacist");
        //    }
        //    return View(pharmacymedication);
        //}





        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AddMedication(PharmacyMedicationModel Model) 
        //{
        //    if (ModelState.IsValid)
        //    {
        //        PharmacyMedicationModel pharmMedModel = new PharmacyMedicationModel
        //        {

        //            MedicationName = Model.MedicationName,  
        //            DosageForm = Model.DosageForm,
        //            Schedule = Model.Schedule,
        //            StockonHand = Model.StockonHand,
        //            ReorderLevel= Model.ReorderLevel,




        //        };

        //      _dbContext.DayHospitalPharmacyMedication.Add(pharmMedModel);
        //       _dbContext.SaveChanges();
        //        return RedirectToAction("AddMedication");
        //    }

        //    return View("AddMedication", Model);
        //}



        // POST: AddMedication
        //[HttpPost]
        //public ActionResult AddMedication(AddMedicationViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Process the form submission here (e.g., save to database)

        //        // Redirect or return a success message
        //        return RedirectToAction("AddMedication"); // Change "Success" to your desired action
        //    }

        //    // If validation fails, fetch data again and return to view with model errors
        //    model.PharmacyMedications = _dbContext.Medication.Select(m => m.MedicationName).ToList();
        //    model.PharmMedDF = _dbContext.Medication.Select(m => m.MedicationForm).ToList();
        //    model.PharmMedSchedule = _dbContext.Medication.Select(m => m.Schedule).ToList();

        //    return View(model);
        //}
















       













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
            // Retrieve the prescription based on the provided ID
          
            var prescription = _dbContext.Prescription.Find(id);


            if (prescription==null)
            {
                return NotFound();
            }

           

            // Combine data from various tables using LINQ
            var combinedData = (from p in _dbContext.Prescription
                                join a in _dbContext.PatientAllergy on p.AdmittedPatientID equals a.PatientID
                                join ai in _dbContext.Activeingredient on a.ActiveingredientID equals ai.ActiveingredientID
                                join c in _dbContext.PatientConditions on p.AdmittedPatientID equals c.PatientID
                                join co in _dbContext.Condition on c.ConditionsID equals co.ConditionID
                                join cm in _dbContext.patientMedication on p.AdmittedPatientID equals cm.PatientID
                                join m in _dbContext.Medication on cm.MedicationID equals m.MedicationID
                                join pv in _dbContext.PatientVitals on p.AdmittedPatientID equals pv.PatientID
                                where p.AdmittedPatientID == id // Ensure we filter by the specific prescription ID
                                select new PharmacistViewScriptModel
                                {
                                    // Medical history
                                    Condition = co.ConditionName,
                                    allergy = ai.ActiveIngredientName,
                                    patientMedication = m.MedicationName,

                                    // Vitals
                                    Height = pv.Height,
                                    Weight = pv.Weight,
                                    SystolicBloodPressure = pv.SystolicBloodPressure,
                                    DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                    HeartRate = pv.HeartRate,
                                    BloodOxygen = pv.BloodOxygen,
                                    Respiration = pv.Respiration,
                                    BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                    Temperature = pv.Temperature,

                                    // Prescription details
                                    Urgency = p.Urgency,
                                    Take = p.Take,
                                    Status = p.Status,
                                    DateGiven = p.DateGiven
                                }).ToList(); // Execute the query and convert to a list

            var currentMed = (from pm in _dbContext.patientMedication
                              join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
                              join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
                              where pm.PatientID == id
                              select new PharmacistViewScriptModel
                              {
                                  patientname = pi.Name,
                                  patientsurname = pi.Surname,
                                  patientMedication = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.patientMedication).ToList();

            var conditions = (from pc in _dbContext.PatientConditions
                              join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
                              join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID

                              select new PharmacistViewScriptModel
                              {
                                  patientname = pt.Name,
                                  patientsurname = pt.Surname,
                                  Condition = c.ConditionName // Ensure this property exists in your view model
                              }).OrderBy(c => c.Condition).ToList();



            var viewModel = new PharmacistViewScriptModel
            {
                combinedData = combinedData,
                AllCurrentMed=currentMed,
               AllConditions=conditions,
                
                
                //Allallergy = allergy,
                //AllConditions = conditions,
                //AllCurrentMed = currentMed





            };
            // Check if any data was retrieved



            //var allergy = (from pa in _dbContext.PatientAllergy
            //               join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
            //               join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID

            //               select new PharmacistViewScriptModel
            //               {
            //                   patientname = p.Name,
            //                   patientsurname = p.Surname,
            //                   allergy = ai.ActiveIngredientName
            //               })
            //.OrderBy(ai => ai.ActiveIngredientName)
            //.ToList();


          


            //var currentMed = (from pm in _dbContext.patientMedication
            //                  join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
            //                  join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
            //                  select new PharmacistViewScriptModel
            //                  {
            //                      patientname = pi.Name,
            //                      patientsurname = pi.Surname,
            //                      patientMedication = cm.MedicationName // Ensure this property exists in your view model
            //                  }).OrderBy(cm => cm.patientMedication).ToList();







            return View(viewModel); // Pass the combined data to the view
    }









    public ActionResult DispenseMedication()
        {

            return View();
        }


        public ActionResult RejectScript()
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

        public ActionResult ViewAllStock()
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
                                join medicationInstruction in _dbContext.MedicationInstructions
                                on prescription.PrescriptionID equals medicationInstruction.PrescriptionID
                                join medication in _dbContext.Medication
                                on medicationInstruction.MedicationID equals medication.MedicationID
                                join patient in _dbContext.PatientInfo
                                on prescription.AccountID equals patient.PatientID // Assuming AccountID is linked to PatientID
                                join account in _dbContext.Accounts
                                on prescription.AccountID equals account.AccountID
                                where prescription.Status=="Prescribed" 

                                select new ViewActivePrescriptionsModel


                                {
                                    // Prescription fields
                                    PrescriptionID=prescription.PrescriptionID,
                                    SurgeonName= account.Name,
                                    SurgeonSurname=account.Surname,
                                    DateGiven = prescription.DateGiven,
                                    Urgency = prescription.Urgency,
                                    Take = prescription.Take,
                                    Status = prescription.Status,

                                    // Medication fields


                                    // MedicationInstructions fields


                                    // PatientInfo fields
                                    Name = patient.Name,
                                    Surname = patient.Surname,



                                }).ToList();

            var sortedData = combinedData.OrderByDescending(item => item.Urgency == "Yes").ToList();

            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);


           
        }



      


























        //Adding my Pharmacy Medication
        //[HttpGet]
        //public ActionResult AddMedication()
        //{
        //    PharmacyMedicationModel pharmacymedication = new PharmacyMedicationModel();
        //    return View(pharmacymedication);
        //}

       






    }
}













    

