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

           





            // Create a ViewModel to hold the data
            var viewModel = new PharmacyMedicationModel
            {


               
                DosageForm=df,
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


        //public IActionResult ViewSpecificPrescription(int id) 
        //{
        //     var patientid = id;
                

        //    return View();
        //}

        public IActionResult ViewSpecificPrescription(int admittedpatientid)
        {





            var patient = _dbContext.AdmittedPatients.FirstOrDefault(ap => ap.AdmittedPatientID == admittedpatientid);

            if (patient == null) 
            { 
                return NotFound();
            }




           var ScriptAndVitals = (from p in _dbContext.Prescription
                                       join ap in _dbContext.AdmittedPatients on p.AdmittedPatientID equals ap.AdmittedPatientID
                                       join pi in _dbContext.PatientInfo on ap.PatientID equals pi.PatientID
                                       join pv in _dbContext.PatientVitals on pi.PatientID equals pv.PatientID

                                       where ap.AdmittedPatientID == admittedpatientid


                                       select new PharmacistViewScriptModel
                                       {
                                           Take = p.Take,
                                           Urgency = p.Urgency,
                                           PrescriptionID = p.PrescriptionID,
                                           PatientID = pi.PatientID,
                                           patientname = pi.Name,
                                           patientsurname = pi.Surname,
                                           Status = p.Status,
                                           Height=pv.Height,
                                           Weight=pv.Weight,
                                           SystolicBloodPressure=pv.SystolicBloodPressure,  
                                           DiastolicBloodPressure=pv.DiastolicBloodPressure,
                                           HeartRate=pv.HeartRate,
                                           BloodOxygen=pv.BloodOxygen,
                                           Respiration=pv.Respiration,
                                           BloodGlucoseLevel=pv.BloodGlucoseLevel,
                                           Temperature=pv.Temperature,
                                           


                                       })

   .ToList();






            var viewModel = new PharmacistViewScriptModel
            {
                PrescrptionDetails= ScriptAndVitals,



            };


            return View(viewModel);



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
                                
                                .ToList();

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













    

