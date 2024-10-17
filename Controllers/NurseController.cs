using DEMO.Data;
using DEMO.Models;
using DEMO.Models.NurseModels;
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
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace DEMO.Controllers
{
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private int _bookingId = 0;

        public NurseController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult MainPage(int accountId)
        {

            var nurse = _dbContext.Accounts
                .Where(a => a.AccountID == accountId && a.Role == "Nurse")
                .Select(a => new NurseView
                {
                    AccountID = a.AccountID,
                    Name = a.Name,
                    Surname = a.Surname,
                    Email = a.Email
                })
                .SingleOrDefault();

            if (nurse == null)
            {
                return NotFound();
            }

            // Store critical user data in session
            HttpContext.Session.SetString("UserAccountId", nurse.AccountID.ToString());
            HttpContext.Session.SetString("UserName", nurse.Name);
            HttpContext.Session.SetString("UserSurname", nurse.Surname);
            HttpContext.Session.SetString("UserEmail", nurse.Email);

            // Optionally, you can use ViewBag for non-critical or UI-specific data
            ViewBag.AccountID = nurse.AccountID;
            ViewBag.UserName = nurse.Name;
            ViewBag.UserSurname = nurse.Surname;
            ViewBag.Email = nurse.Email;

            return View();
        }


        public IActionResult Vitals()
        {
            return View();

        }
        
        public IActionResult Discharge()
        {
            return View();
        }
        public IActionResult DischargeList()
        {
            //var combinedData = (from bs in _dbContext.BookSurgery
            //                    join a in _dbContext.Accounts
            //                    on bs.AccountID equals a.AccountID
            //                    join p in _dbContext.PatientInfo
            //                    on bs.PatientID equals p.PatientID
            //                    select new ViewBookings
            //                    {
            //                        BookingID = bs.BookingID,
            //                        AccountName = a.Name,
            //                        AccountSurname = a.Surname,
            //                        PatientName = p.Name,
            //                        PatinetSurname = p.Surname,
            //                        PatientID = bs.PatientID,
            //                        SurgeryTime = bs.SurgeryTime,
            //                        SurgeryDate = bs.SurgeryDate,
            //                        Theater = bs.Theater
            //                    }).OrderBy(a => a.AccountName).ToList();
            return View();
        }
        public IActionResult ViewSurgeryBooking()
        {
            var combinedData = (from bs in _dbContext.BookSurgery
                                join a in _dbContext.Accounts
                                on bs.AccountID equals a.AccountID
                                join p in _dbContext.PatientInfo
                                on bs.PatientID equals p.PatientID
                                select new ViewBookings
                                {
                                    BookingID = bs.BookingID,
                                    AccountName = a.Name,
                                    AccountSurname = a.Surname,
                                    PatientName = p.Name,
                                    PatientSurname = p.Surname,
                                    PatientID = bs.PatientID,
                                    SurgeryTime = bs.SurgeryTime,
                                    SurgeryDate = bs.SurgeryDate,
                                    Theater = bs.Theater
                                }).OrderBy(a => a.AccountName).ToList();

            var viewModel = new ViewBookings
            {
                AllcombinedData = combinedData,

            };
            return View(viewModel);
            //return RedirectToAction("AdmissionPage", new { id = viewModel.BookingID});
            
        }

        
        [HttpPost]
        public IActionResult AdmissionPage(string ConditionsJson,string AllergiesJson, string MedicationJson, BookedPatientInfo model)
        {
            int patientAllergyID = 0;
            int patientConditionID = 0;
            int patientMedicationId = 0;
            int pationetAddressId = 0;
            int bookingId = 0;

            if (TempData["BookingId"] != null)
            {
                bookingId = (int)TempData["BookingId"];
            }

            if (!string.IsNullOrEmpty(ConditionsJson))
            {
                //model.Conditions = JsonConvert.DeserializeObject<List<Condition>>(ConditionsJson);
                var list = JsonConvert.DeserializeObject<List<string>>(ConditionsJson);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        Condition condition = new Condition();
                        condition.ConditionID = _dbContext.Condition.Where(x => x.ConditionName == item.ToString()).FirstOrDefault().ConditionID;

                        model.Conditions.Add(condition);
                    }
                }
            }

            if (!string.IsNullOrEmpty(AllergiesJson))
            {
                //model.Conditions = JsonConvert.DeserializeObject<List<Condition>>(ConditionsJson);
                var allergylist = JsonConvert.DeserializeObject<List<string>>(AllergiesJson);

                if (allergylist != null)
                {
                    foreach (var item in allergylist)
                    {
                        Activeingredient allergy = new Activeingredient();
                        allergy.ActiveingredientID = _dbContext.Activeingredient.Where(x => x.ActiveIngredientName == item.ToString()).FirstOrDefault().ActiveingredientID;

                        model.Allergies.Add(allergy);
                    }
                }
            }

            if (!string.IsNullOrEmpty(MedicationJson))
            {
                //model.Conditions = JsonConvert.DeserializeObject<List<Condition>>(ConditionsJson);
                var medicationlist = JsonConvert.DeserializeObject<List<string>>(MedicationJson);

                if (medicationlist != null)
                {
                    foreach (var item in medicationlist)
                    {
                        Medication medication = new Medication();
                        medication.MedicationID = _dbContext.Medication.Where(x => x.MedicationName == item.ToString()).FirstOrDefault().MedicationID;

                        model.Medications.Add(medication);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                var booking = _dbContext.BookSurgery.Where(x => x.BookingID == bookingId).FirstOrDefault();

                if (booking != null && booking.PatientID > 0)
                {
                    var patientInfo = new AdmittedPatientsModel()
                    {
                        PatientID = booking.PatientID,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        AdmissionStatusID = _dbContext.AdmissionStatus.Where(x => x.Description == "Admitted").FirstOrDefault().AdmissionStatusId,
                    };

                    _dbContext.AdmittedPatients.Add(patientInfo);
                    _dbContext.SaveChanges();

                    int admissionId = patientInfo.AdmittedPatientID;

                    if (admissionId > 0)
                    {
                        var address = new Address();
                        var updateAdmission = patientInfo;

                        if (model.Province != null)
                            address.ProvinceID = model.Province.ProvinceID;
                        if (model.City != null)
                            address.CityID = model.City.CityID;
                        if (model.Suburb != null)
                            address.SuburbID = model.Suburb.SuburbID;

                        if (model.Province != null || model.City != null || model.Suburb != null || model.Street != null)
                        {
                            var patientAddress = new Address()
                            {
                                ProvinceID = model.Province.ProvinceID,
                                CityID = model.City.CityID,
                                SuburbID = model.Suburb.SuburbID,
                                StreetName = model.Street,
                                
                            };

                            _dbContext.Address.Add(patientAddress);
                            _dbContext.SaveChanges();

                            pationetAddressId = patientAddress.AddressId;
                        }

                        if (model.Allergies != null && model.Allergies.Any())
                        {
                            foreach (var item in model.Allergies)
                            {
                            var allergies = new PatientAllergy()
                            {
                                PatientID = booking.PatientID,
                                ActiveingredientID = item.ActiveingredientID,
                                
                            };

                                _dbContext.PatientAllergy.Add(allergies);
                                _dbContext.SaveChanges();

                                patientAllergyID = allergies.patientAllergyID;
                            }
                        }
                        

                        if(model.Conditions != null && model.Conditions.Any())
                        {
                            foreach (var item in model.Conditions)
                            {
                                var condition = new PatientConditions()
                                {
                                    PatientID = booking.PatientID,
                                    ConditionsID = item.ConditionID,
                                    
                                };

                                _dbContext.PatientConditions.Add(condition);
                                _dbContext.SaveChanges();

                                patientConditionID = condition.PatientConditionsID;
                            }
                        }

                        if(model.Medications != null && model.Medications.Any())
                        {
                            foreach (var item in model.Medications)
                            {
                                var medication = new PatientMedication()
                                {
                                    PatientID = booking.PatientID,
                                    MedicationID = item.MedicationID,
                                    
                                };

                                _dbContext.patientMedication.Add(medication);
                                _dbContext.SaveChanges();

                                patientMedicationId = medication.PatientMedicationID;
                            }

                        }
                        
                        double HeightinM = model.Vitals.Height / 100;
                        double BMI = Math.Round(model.Vitals.Weight / (HeightinM * HeightinM));
                        if (model.Vitals != null)
                        {
                            var patientVitals = new PatientVitals
                            {
                                PatientID = booking.PatientID,
                                Height = model.Vitals.Height,
                                Weight = model.Vitals.Weight,
                                SystolicBloodPressure = model.Vitals.SystolicBloodPressure,
                                DiastolicBloodPressure = model.Vitals.DiastolicBloodPressure,
                                HeartRate = model.Vitals.HeartRate,
                                BloodOxygen = model.Vitals.BloodOxygen,
                                Respiration = model.Vitals.Respiration,
                                BloodGlucoseLevel = model.Vitals.BloodGlucoseLevel,
                                Temperature = model.Vitals.Temperature,
                                time = model.Vitals.time,
                            };
                            
                            if (model.Vitals.SystolicBloodPressure > 36)
                            {

                            }
                            if (model.Vitals.DiastolicBloodPressure > 36)
                            {

                            }
                            if (model.Vitals.HeartRate > 36)
                            {


                            }
                            if (model.Vitals.BloodOxygen > 36)
                            {

                            }
                            if (model.Vitals.BloodGlucoseLevel > 36)
                            {

                            }
                            if (model.Vitals.Respiration > 36)
                            {

                            }
                            if (model.Vitals.Temperature > 36)
                            {

                            }
                            _dbContext.PatientVitals.Add(patientVitals);
                            _dbContext.SaveChanges();

                            int vitalsId = patientVitals.PatientVitalsID;

                            if(vitalsId > 0)
                            {
                                updateAdmission.BookingID = bookingId;
                                updateAdmission.WardID = model.Ward.WardId;
                                updateAdmission.BedId = model.Bed.BedId;
                                updateAdmission.AddressID = pationetAddressId;

                                _dbContext.AdmittedPatients.Update(updateAdmission);
                                _dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Vitals has not been created");
                        }

                        
                    }
                    else
                    {
                        Console.WriteLine("Admision has not been created");
                    }
                }
                else
                {
                    Console.WriteLine("Patient not linked to a booking ID");
                }
                //_dbContext.AdmittedPatients.Add(model);
                //_dbContext.SaveChanges();
                //return RedirectToAction("AdmittedPatients");
            }

            // Return the view with the model if validation fails
            return View("AdmittedPatients");
        }

        public IActionResult AdmittedPatients()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                accountID = 0;
            }

            var combinedData = (from a in _dbContext.Accounts
                                join b in _dbContext.BookSurgery on a.AccountID equals b.AccountID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join ap in _dbContext.AdmittedPatients on b.BookingID equals ap.BookingID
                                join bed in _dbContext.Bed on ap.BedId equals bed.BedId
                                join w in _dbContext.Ward on bed.WardID equals w.WardId
                                join status in _dbContext.AdmissionStatus on ap.AdmissionStatusID equals status.AdmissionStatusId
                               
                                select new AdmissionsListViewModel
                                {
                                    PatientID = p.PatientID,
                                    AdmittedPatientID = ap.AdmittedPatientID,
                                    BookingID = b.BookingID,
                                    SurgeonName = a.Name,
                                    SurgeonSurname = a.Surname,
                                    Name = p.Name,
                                    Surname = p.Surname,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater,
                                    WardName = w.WardName,
                                    BedNumber = bed.Number,
                                    AdmissionStatusDescription = status.Description,
                                    Time = ap.Time
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

            
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            //var patients = _dbContext.AdmittedPatients.ToList();
            return View("AdmittedPatients",viewModel);
        }

        public IActionResult AdmissionPage(int bookingID)
        {
            TempData["BookingId"] = bookingID;

            var booking = _dbContext.BookSurgery
                .Where(b => b.BookingID == bookingID)
                .Join(
                    _dbContext.PatientInfo,
                    surgery => surgery.PatientID,
                    patient => patient.PatientID,
                    (surgery, patient) => new BookedPatientInfo
                    {
                        Name = patient.Name,
                        Surname = patient.Surname,
                        Date = surgery.SurgeryDate
                    })
                .FirstOrDefault();

            if (booking == null)
            {
                return NotFound();
            }

            // Set the ViewBag properties for the view
            ViewBag.PatientName = $"{booking.Name} {booking.Surname}";
            ViewBag.SurgeryDate = booking.Date.ToString("yyyy-MM-dd"); // Format the date as needed


            ViewBag.UserName = booking.Name;
            ViewBag.UserSurname = booking.Surname;
            ViewBag.UserEmail = booking.Date;

            return View(booking);
        }
        public IActionResult MedicationCollection()
        {
            return View();
        }
        public IActionResult MedicationAdministration()
        {
            return View();
        }
        public IActionResult PatientRecords(int AdmittedPatientID)
        {
            var combinedData = (from a in _dbContext.Accounts
                                join b in _dbContext.BookSurgery on a.AccountID equals b.AccountID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join ap in _dbContext.AdmittedPatients on p.PatientID equals ap.PatientID
                                join bed in _dbContext.Bed on ap.BedId equals bed.BedId
                                join w in _dbContext.Ward on bed.WardID equals w.WardId
                                join status in _dbContext.AdmissionStatus on ap.AdmissionStatusID equals status.AdmissionStatusId
                                where ap.AdmittedPatientID == AdmittedPatientID

                                select new AdmissionsListViewModel
                                {
                                    PatientID = p.PatientID,
                                    AdmittedPatientID = ap.AdmittedPatientID,
                                    BookingID = b.BookingID,
                                    SurgeonName = a.Name,
                                    SurgeonSurname = a.Surname,
                                    Name = p.Name,
                                    Gender = p.Gender,
                                    Surname = p.Surname,
                                    SurgeryDate = b.SurgeryDate,
                                    SurgeryTime = b.SurgeryTime,
                                    Theater = b.Theater,
                                    WardName = w.WardName,
                                    BedNumber = bed.Number,
                                    AdmissionStatusDescription = status.Description,
                                    Time = ap.Time
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


            
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        public IActionResult PatientVitals(int AdmittedPatientID, string name, string surname)
        {
            var patientVitals = (from pv in _dbContext.PatientVitals
                                 join ap in _dbContext.AdmittedPatients
                                 on pv.PatientID equals ap.PatientID
                                 where ap.AdmittedPatientID == AdmittedPatientID

                                 select new PatientAllergyViewModel
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


            var allergy = (from pa in _dbContext.PatientAllergy
                           join ap in _dbContext.AdmittedPatients on pa.PatientID equals ap.PatientID
                           join p in _dbContext.PatientInfo on pa.PatientID equals p.PatientID
                           join ai in _dbContext.Activeingredient on pa.ActiveingredientID equals ai.ActiveingredientID
                           where ap.AdmittedPatientID == AdmittedPatientID
                           select new PatientAllergyViewModel
                           {
                               Name = p.Name,
                               Surname = p.Surname,
                               ActiveIngredientName = ai.ActiveIngredientName
                           })
            .OrderBy(ai => ai.ActiveIngredientName)
            .ToList();


            var conditions = (from pc in _dbContext.PatientConditions
                              join ap in _dbContext.AdmittedPatients on pc.PatientID equals ap.PatientID
                              join pt in _dbContext.PatientInfo on pc.PatientID equals pt.PatientID
                              join c in _dbContext.Condition on pc.ConditionsID equals c.ConditionID
                              where ap.AdmittedPatientID == AdmittedPatientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pt.Name,
                                  Surname = pt.Surname,
                                  ConditionName = c.ConditionName // Ensure this property exists in your view model
                              }).OrderBy(c => c.ConditionName).ToList();

            var currentMed = (from pm in _dbContext.patientMedication
                              join ap in _dbContext.AdmittedPatients on pm.PatientID equals ap.PatientID
                              join cm in _dbContext.Medication on pm.MedicationID equals cm.MedicationID
                              join pi in _dbContext.PatientInfo on pm.PatientID equals pi.PatientID
                              where ap.AdmittedPatientID == AdmittedPatientID
                              select new PatientAllergyViewModel
                              {
                                  Name = pi.Name,
                                  Surname = pi.Surname,
                                  MedicationName = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.MedicationName).ToList();
            // Create a view model that holds both lists
            var viewModel = new PatientAllergyViewModel
            {
                Allvitals = patientVitals,
                Allallergy = allergy,
                AllConditions = conditions,
                AllCurrentMed = currentMed
            };

            ViewBag.PatientName = name;
            ViewBag.PatientSurname = surname;

            return View(viewModel);
           


         
         
         
            
        }
        
        public IActionResult InfoNurse()
        {
            return View();
        }
        public IActionResult EmailVitals()
        {
            return View();
        }
        public async Task<IActionResult> SendEmailVitals(string notes, int patientID)
        {
            // Get logged-in nurse's email
            
            
                // Get logged-in nurse's email
                var nurseEmail = User?.Identity?.Name;

                if (nurseEmail == null)
                {
                    TempData["ErrorMessage"] = "Could not retrieve the nurse's email.";
                    return RedirectToAction("AdmittedPatients", "Nurse");
                }

                // Fetch the latest vitals and related data for the specific patient
                var combinedData = await (from pv in _dbContext.PatientVitals
                                          join ad in _dbContext.AdmittedPatients on pv.PatientID equals ad.PatientID
                                          join b in _dbContext.BookSurgery on ad.BookingID equals b.BookingID
                                          join ac in _dbContext.Accounts on b.AccountID equals ac.AccountID
                                          join p in _dbContext.PatientInfo on ad.PatientID equals p.PatientID
                                          join w in _dbContext.Ward on ad.WardID equals w.WardId
                                          join bed in _dbContext.Bed on ad.BedId equals bed.BedId
                                          where pv.PatientID == patientID
                                          orderby pv.time descending // Get the latest vitals
                                          select new EmailVital
                                          {
                                              Height = pv.Height,
                                              Weight = pv.Weight,
                                              SystolicBloodPressure = pv.SystolicBloodPressure,
                                              DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                              HeartRate = pv.HeartRate,
                                              BloodOxygen = pv.BloodOxygen,
                                              Respiration = pv.Respiration,
                                              BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                              Temperature = pv.Temperature,
                                              Time = pv.time, // Format the time
                                              FullName = p.Name + " " + p.Surname,
                                              WardName = w.WardName,
                                              Bed = bed.Number,
                                              SurgeonEmail = ac.Email,
                                              SurgeonRole = ac.Role
                                          }).FirstOrDefaultAsync(); // Get only the latest record

                if (combinedData == null )
                {
                    TempData["ErrorMessage"] = "Vitals or patient not found.";
                    return RedirectToAction("AdmittedPatients", "Nurse");
                }

                if (string.IsNullOrEmpty(combinedData.SurgeonEmail))
                {
                    TempData["ErrorMessage"] = "Surgeon's email not found.";
                    return RedirectToAction("AdmittedPatients", "Nurse");
                }

                // Prepare email content
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Day Hospital - Apollo+(Group 9 - 4Year)", nurseEmail));
                emailMessage.To.Add(new MailboxAddress("Surgeon", combinedData.SurgeonEmail));
                emailMessage.Subject = $"Latest Vitals for Patient {combinedData.FullName}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h3>Patient Information</h3>
                        <p><strong>Full Name:</strong> {combinedData.FullName} </p>
                        <p><strong>Ward Name:</strong> {combinedData.WardName} </p>
                        <p><strong>Bed:</strong> {combinedData.Bed} </p>
                        
                        <h3>Latest Vitals:</h3>
                        <ul>
                            <li>Height: {combinedData.Height} cm</li>
                            <li>Weight: {combinedData.Weight} kg</li>
                            <li>Blood Pressure: {combinedData.SystolicBloodPressure}/{combinedData.DiastolicBloodPressure} mmHg</li>
                            <li>Heart Rate: {combinedData.HeartRate} bpm</li>
                            <li>Blood Oxygen: {combinedData.BloodOxygen}%</li>
                            <li>Respiration: {combinedData.Respiration} bpm</li>
                            <li>Blood Glucose Level: {combinedData.BloodGlucoseLevel} mg/dL</li>
                            <li>Temperature: {combinedData.Temperature}°C</li>
                            <li>Time: {combinedData.Time}</li>
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
                        await client.AuthenticateAsync(Environment.GetEnvironmentVariable("SMTP_EMAIL"), Environment.GetEnvironmentVariable("SMTP_PASSWORD"));
                        await client.SendAsync(emailMessage);
                        await client.DisconnectAsync(true);
                    }

                    TempData["SuccessMessage"] = "Email sent successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error sending email: {ex.Message}";
                }

                return RedirectToAction("AdmittedPatients", "Nurse");
            }




            [HttpPost]
        public IActionResult SubmitForm(BookedPatientInfo model)
        {
            

            if (ModelState.IsValid)
            {
                // Handle form submission
                // 'model.Province' will be bound from the selected ProvinceId in the form
                // 'model.City' will be bound from the selected CityId in the form

                // Example: Save model to the database or perform other actions
            }

            // If the model state is invalid or you want to re-render the form
            return View(model);
        }

        //        public IActionResult EmailVitals(int id)
        //        {
        //            // Fetch the user based on AccountID
        //            var user = _dbContext.Accounts
        //                                   .FirstOrDefault(p => p.AccountID == id);
        //            if (user == null)
        //            {
        //                return NotFound(); // Return 404 if user is not found
        //            }

        //            // Prepare the view model
        //            var viewModel = new EmailVital
        //            {
        //                AccountID = user.AccountID,
        //                FullName = $"{user.Name} {user.Surname}",
        //                Email = user.Email,
        //                Vitals
        //                Notes = string.Empty // Initial empty notes
        //            };

        //            return View(viewModel); // Return the view with the model
        //        }

        //        //FIX EMAIL!!!!
        //        [HttpPost]
        //        public async Task<IActionResult> EmailVitals(int id, string notes)
        //        {
        //            var user = _dbContext.Accounts
        //                                  .FirstOrDefault(p => p.AccountID == id);

        //            var emailMessage = new MimeMessage();
        //            emailMessage.From.Add(new MailboxAddress("Day Hospital - Apollo+", "noreply@dayhospital.com"));
        //            emailMessage.To.Add(new MailboxAddress(user.Role, user.Email));
        //            emailMessage.Subject = "Vitals Concern";

        //            var bodyBuilder = new BodyBuilder
        //            {
        //                HtmlBody = $@"
        //         <h3>User Information</h3>
        //         <p><strong>Name:</strong> {user.Name} {user.Surname}</p>
        //         <h3>Account Has Been Added</h3>
        //</n>
        //         <p><strong>Username:</strong> {user.Username}</p>
        //         <p><strong>Password:</strong> {user.Password}</p>        
        //</n>
        //         <h3>Notes</h3>
        //         <p>{notes}</p>"
        //            };

        //            emailMessage.Body = bodyBuilder.ToMessageBody();

        //            using (var client = new MailKit.Net.Smtp.SmtpClient())
        //            {
        //                client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

        //                client.Authenticate("jansen.ronaldocullen@gmail.com", "xqqx kiox hcgm xvmr");
        //                await client.SendAsync(emailMessage);
        //                client.Disconnect(true);

        //            }

        //            TempData["SuccessMessage"] = "Email sent successfully.";
        //            return RedirectToAction("MainPage", "Nurse");
        //        }

    }
}
