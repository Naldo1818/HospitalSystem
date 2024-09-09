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
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                        Date = DateTime.Now,
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

                        if (model.Province != null || model.City != null || model.Suburb != null)
                        {
                            var patientAddress = new Address()
                            {
                                ProvinceID = model.Province.ProvinceID,
                                CityID = model.City.CityID,
                                SuburbID = model.Suburb.SuburbID,
                            };

                            _dbContext.Address.Add(patientAddress);
                            _dbContext.SaveChanges();

                            pationetAddressId = patientAddress.AddressId;
                        }
                        

                        foreach (var item in model.Allergies)
                        {
                            var allergies = new PatientAllergy()
                            {
                                PatientID = booking.PatientID,
                                ActiveingredientID = item.ActiveingredientID,
                                AdmittedPatientID = admissionId,
                            };

                            _dbContext.PatientAllergy.Add(allergies);
                            _dbContext.SaveChanges();

                            patientAllergyID = allergies.patientAllergyID;
                        }


                        foreach (var item in model.Conditions)
                        {
                            var condition = new PatientConditions()
                            {
                                PatientID = booking.PatientID,
                                ConditionsID = item.ConditionID,
                                AdmittedPatientID = admissionId,
                            };

                            _dbContext.PatientConditions.Add(condition);
                            _dbContext.SaveChanges();

                            patientConditionID = condition.PatientConditionsID;
                        }

                        foreach (var item in model.Medications)
                        {
                            var medication = new PatientMedication()
                            {
                                PatientID = booking.PatientID,
                                CurrentID = item.MedicationID,
                                AdmittedPatientID = admissionId,
                            };

                            _dbContext.patientMedication.Add(medication);
                            _dbContext.SaveChanges();

                            patientMedicationId = medication.PatientMedicationID;
                        }

                        if (model.Vitals != null)
                        {
                            var patientVitals = new PatientVitals
                            {
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

                            _dbContext.PatientVitals.Add(patientVitals);
                            _dbContext.SaveChanges();

                            int vitalsId = patientVitals.PatientVitalsID;

                            if(vitalsId > 0)
                            {
                                updateAdmission.PatientVitalsID = vitalsId;
                                updateAdmission.BookingID = bookingId;
                                updateAdmission.WardID = model.Ward.WardId;
                                updateAdmission.PatientDetailsID = pationetAddressId;

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
            return View("AdmittedPatients", model);
        }

        public IActionResult AdmittedPatients()
        {
            //// Return the list of admitted patients or a relevant view
            //var patients = _dbContext.AdmittedPatients.ToList();
            return View();
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
        public IActionResult PatientRecords()
        {
            return View();
        }
        public IActionResult PatientVitals()
        {
            return View();
        }
        public IActionResult PatientConditions()
        {
            return View();
        }
        public IActionResult PatientMedication()
        {
            return View();
        }
        public IActionResult PatientAllergies()
        {
            return View();
        }
        public IActionResult InfoNurse()
        {
            return View();
        }
        public IActionResult EmailVitals()
        {
            return View();
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
