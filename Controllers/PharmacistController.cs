using DEMO.Data;
using DEMO.Models;
using DEMO.Models.NurseModels;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using MimeKit;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            var prescription = _dbContext.Prescription.Find(id);

            var combinedData = (from p in _dbContext.Prescription
                                join ad in _dbContext.AdmittedPatients
                                        on p.AdmittedPatientID equals ad.AdmittedPatientID

                                join a in _dbContext.PatientAllergy
                                on ad.PatientID equals a.PatientID
                                join ai in _dbContext.Activeingredient
                                on a.ActiveingredientID equals ai.ActiveingredientID


                                join c in _dbContext.PatientConditions
                                on ad.PatientID equals c.PatientID
                                join co in _dbContext.Condition
                                on c.ConditionsID equals co.ConditionID

                                join cm in _dbContext.patientMedication
                                on ad.PatientID equals cm.PatientID
                                join m in _dbContext.Medication
                                on cm.MedicationID equals m.MedicationID

                                join pv in _dbContext.PatientVitals
                                on ad.PatientID equals pv.PatientID

                              

                                select new PharmacistViewScriptModel
                                { 
                                    //medical history
                                    Condition = co.ConditionName,
                                    allergy=ai.ActiveIngredientName,
                                    patientMedication= m.MedicationName,

                                    //vitals
                                    Height=pv.Height,
                                    Weight=pv.Weight,
                                    SystolicBloodPressure=pv.SystolicBloodPressure,
                                    DiastolicBloodPressure=pv.DiastolicBloodPressure,
                                    HeartRate=pv.HeartRate,
                                    BloodOxygen=pv.BloodOxygen,
                                    Respiration=pv.Respiration,
                                    BloodGlucoseLevel=pv.BloodGlucoseLevel,
                                    Temperature=pv.Temperature,

                                    //prescription
                                    Urgency=p.Urgency,
                                    Take=p.Take,
                                    Status=p.Status,
                                    DateGiven=p.DateGiven,



                                   
        //public DateOnly DateGiven { get; set; }

        
        //public string Urgency { get; set; }
       
        //public string Take { get; set; }

       
        //public string Status { get; set; }






    }              
                                );

           

            return View();

          
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

        //[HttpPost]
        //[ValidateAntiForgeryToken]

        //public async Task<IActionResult> AddMedication([Bind

        //    ("PharmacyMedicationlID,MedicationName,DosageForm,Schedule,StockonHand,ReorderLevel,ActiveIngredientsAndStrength")]
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







    }
}













    

