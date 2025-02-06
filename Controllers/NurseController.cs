using DEMO.Data;
using DEMO.Models;
using DEMO.Models.NurseModels;
using DEMO.ViewModels;
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

        public IActionResult MainPage(int accountID)
        {

            var nurse = _dbContext.Accounts
                .Where(a => a.AccountID == accountID && a.Role == "Nurse")
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

        
        [HttpPost]
        public IActionResult Discharge(int admittedPatientID)
        {
            // The CSRF validation is now removed
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data provided for discharge.";
                return RedirectToAction("AdmittedPatients");
            }

            var admittedPatient = _dbContext.AdmittedPatients.FirstOrDefault(ap => ap.AdmittedPatientID == admittedPatientID);

            if (admittedPatient == null)
            {
                TempData["ErrorMessage"] = "Patient not found.";
                return RedirectToAction("AdmittedPatients");
            }

            admittedPatient.AdmissionStatusID = 2; // Assuming 2 means "discharged"
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Patient has been successfully discharged.";
            return RedirectToAction("AdmittedPatients");
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
        public IActionResult ViewSurgeryBooking(int accountID)
        {
            var combinedData = (from bs in _dbContext.BookSurgery
                                join a in _dbContext.Accounts on bs.AccountID equals a.AccountID
                                join p in _dbContext.PatientInfo on bs.PatientID equals p.PatientID
                                join ad in _dbContext.AdmittedPatients on bs.BookingID equals ad.BookingID into adGroup
                                from ad in adGroup.DefaultIfEmpty() // This allows us to get data even if there is no admission record
                                join sta in _dbContext.AdmissionStatus on ad.AdmissionStatusID equals sta.AdmissionStatusId into statusGroup
                                from sta in statusGroup.DefaultIfEmpty() // Ensures we get the status even if the patient isn't admitted
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
                                    Theater = bs.Theater,
                                    AdmissionStatus = (sta != null) ? sta.Description : null, // Handle null manually
                                    IsAdmitted = ad != null // True if the patient is admitted
                                }).OrderBy(a => a.AccountName).ToList();

            var viewModel = new ViewBookings
            {
                AllcombinedData = combinedData,
            };
            ViewBag.AccountID = accountID;
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult AdmissionPage(string ConditionsJson, string AllergiesJson, string MedicationJson, BookedPatientInfo model)
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                accountID = 0;
            }

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
                        AccountID = accountID,
                        PatientID = booking.PatientID,
                        Date = DateOnly.FromDateTime(DateTime.Now),
                        Height = model.Height,
                        Weight = model.Weight,
                        AdmissionStatusID = _dbContext.AdmissionStatus.Where(x => x.Description == "Admitted").FirstOrDefault().AdmissionStatusId,
                        Time = TimeOnly.FromDateTime(DateTime.Now)
                    };
                        double HeightinM = patientInfo.Height/ 100;
                        double BMI = Math.Round(patientInfo.Weight / (HeightinM * HeightinM));
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


                        if (model.Conditions != null && model.Conditions.Any())
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

                        if (model.Medications != null && model.Medications.Any())
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

                        
                        if (model.Vitals != null)
                        {
                            var patientVitals = new PatientVitals
                            {
                                PatientID = booking.PatientID,
                                SystolicBloodPressure = model.Vitals.SystolicBloodPressure,
                                DiastolicBloodPressure = model.Vitals.DiastolicBloodPressure,
                                HeartRate = model.Vitals.HeartRate,
                                BloodOxygen = model.Vitals.BloodOxygen,
                                Respiration = model.Vitals.Respiration,
                                BloodGlucoseLevel = model.Vitals.BloodGlucoseLevel,
                                Temperature = model.Vitals.Temperature,
                                time = TimeOnly.FromDateTime(DateTime.Now),
                            };

                           
                            _dbContext.PatientVitals.Add(patientVitals);
                            _dbContext.SaveChanges();

                            int vitalsId = patientVitals.PatientVitalsID;

                            if (vitalsId > 0)
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
                return RedirectToAction("AdmittedPatients", "Nurse");
            }

            // Return the view with the model if validation fails
            return View("AdmittedPatients", model);
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
                                where ap.AccountID == accountID && ap.AdmissionStatusID == 1

                                select new BookedPatientInfo
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
            var viewModel = new BookedPatientInfo
            {
                AllcombinedData = combinedData,

            };
            
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            ViewBag.AccountID = accountID;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
        

            //var patients = _dbContext.AdmittedPatients.ToList();
            return View("AdmittedPatients", viewModel);
        }

        public IActionResult AdmissionPage(int bookingID, int accountID)
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
            ViewBag.AccountID = accountID;

            ViewBag.UserName = booking.Name;
            ViewBag.UserSurname = booking.Surname;
            ViewBag.UserEmail = booking.Date;

            return View(booking);
        }
        public IActionResult MedicationCollection(string name, string surname, string ward, int bed, int admittedPatientId, int accountID)
        {
            // Fetch dispensed prescriptions along with patient information
            var combinedData = (from p in _dbContext.PatientInfo
                                join ap in _dbContext.AdmittedPatients on p.PatientID equals ap.PatientID
                                join pr in _dbContext.Prescription on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                where pr.Status == "Dispensed" && pr.AdmittedPatientID == admittedPatientId
                                orderby pr.Urgency == "Yes" descending, p.Name
                                select new PrescriptionListViewModal
                                {
                                    PrescriptionID = pr.PrescriptionID,
                                    PatientName = p.Name,
                                    PatientSurname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Urgency = pr.Urgency,
                                    Status = pr.Status
                                }).ToList();

            // Retrieve user information from the session
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            // Populate ViewBag with necessary information
            ViewBag.AccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.PatientWard = ward;
            ViewBag.PatientBed = bed;
            ViewBag.AdmittedPatientID = admittedPatientId;
            ViewBag.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Create a ViewModel instance and pass the data to the view
            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribedDispensed = combinedData
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CollectMedication(int prescriptionId, string name, string surname, string ward, int bed, int admittedPatientId, int accountID)
        {
            // Fetch the prescription record
            var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == prescriptionId);

            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }

            // Update the status to "Collected"
            prescription.Status = "Collected";
            _dbContext.SaveChanges();

            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            ViewBag.AccountID = accountID;
            ViewBag.AdmittedPatientID = admittedPatientId;
            ViewBag.PrescriptionID = prescriptionId;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.PatientWard = ward;
            ViewBag.PatientBed = bed;
            ViewBag.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Redirect to the Medication Administration page

            return RedirectToAction("MedicationCollected", new
            {
                prescriptionID = prescriptionId,
                name = name,
                surname = surname,
                ward = ward,
                bed = bed,
                admittedPatientId = admittedPatientId,
                accountID = accountID
            });
        }

        public IActionResult MedicationCollected(int prescriptionID,string name, string surname, string ward, int bed, int admittedPatientId, int accountID)
        {
            // Fetch dispensed prescriptions along with patient information
            var combinedData = (from p in _dbContext.PatientInfo
                                join ap in _dbContext.AdmittedPatients on p.PatientID equals ap.PatientID
                                join pr in _dbContext.Prescription on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                where pr.Status == "Collected" && pr.AdmittedPatientID == admittedPatientId
                                select new PrescriptionListViewModal
                                {
                                    PrescriptionID = pr.PrescriptionID,
                                    PatientName = p.Name,
                                    PatientSurname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Urgency = pr.Urgency,
                                    Status = pr.Status
                                }).ToList();

            // Retrieve user information from the session
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            // Populate ViewBag with necessary information
            ViewBag.AccountID = accountID;
            ViewBag.AdmittedPatientID = admittedPatientId;
            ViewBag.PrescriptionID = prescriptionID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.PatientWard = ward;
            ViewBag.PatientBed = bed;
            ViewBag.Time = TimeOnly.FromDateTime(DateTime.Now);

            // Create a ViewModel instance and pass the data to the view
            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribedDispensed = combinedData
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CollectedMedication(int prescriptionId, string name, string surname, string ward, int bed, int admittedPatientId, int accountID)
        {
            // Fetch the prescription record
            var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == prescriptionId);

            if (prescription == null)
            {
                return NotFound("Prescription not found.");
            }

            // Redirect to the Medication Administration page with parameters
            return RedirectToAction("MedicationAdministration", new
            {
                prescriptionID = prescriptionId,
                name = name,
                surname = surname,
                ward = ward,
                bed = bed,
                admittedPatientId = admittedPatientId,
                accountID = accountID
            });
        }

        public IActionResult MedicationAdministration(int prescriptionID, string name, string surname, string ward, int bed, int admittedPatientId, int accountID, DateOnly date)
        {
            // Fetch the collected prescriptions along with patient and medication details
            var combinedData = (from p in _dbContext.PatientInfo
                                join ap in _dbContext.AdmittedPatients
                                on p.PatientID equals ap.PatientID
                                join pr in _dbContext.Prescription
                                on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                join mis in _dbContext.MedicationInstructions
                                on pr.PrescriptionID equals mis.PrescriptionID
                                join meds in _dbContext.Medication
                                on mis.MedicationID equals meds.MedicationID
                                where pr.PrescriptionID == prescriptionID // Ensure status is 'Collected'
                                select new PrescriptionListViewModal
                                {
                                    PrescriptionID = pr.PrescriptionID,
                                    PatientName = p.Name,
                                    PatientSurname = p.Surname,
                                    DateGiven = pr.DateGiven,
                                    Status = pr.Status,
                                    MedicationName = meds.MedicationName,
                                    Instructions = mis.Instructions,
                                    Quantity = mis.Quantity,
                                    MedicationID = meds.MedicationID
                                }).ToList();

            // Fetch already administered quantities for the specific patient and prescription
            var administeredQuantities = (from am in _dbContext.AdministerMedication
                                          where am.AdmittedPatientID == admittedPatientId && am.PrescriptionID == prescriptionID
                                          group am by am.MedicationID into g
                                          select new
                                          {
                                              MedicationID = g.Key,
                                              TotalAdministered = g.Sum(x => x.AdministerQuantity)
                                          }).ToList();



            // Retrieve user information from the session
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.PatientWard = ward;
            ViewBag.PatientBed = bed;
            ViewBag.Date = date;
            ViewBag.AdmittedPatientID = admittedPatientId;
            ViewBag.AccountID = accountID;

            // Create a ViewModel instance and pass the data to the view
            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribedDispensed = combinedData // Populate collected prescriptions
            };

            if (prescriptionID > 0)
            {
                viewModel.PrescriptionID = prescriptionID;
            }

            return View(viewModel);
        }


        [HttpPost]
        public IActionResult AdministerMedication(int prescriptionID, string name, string surname, string ward, int bed, int admittedPatientId, int accountID, string medicationsData)
        {
            // Deserialize the medications data
            var medications = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdministeredMedicationViewModel>>(medicationsData);

            if (medications == null || !medications.Any())
            {
                return RedirectToAction("Error", "Home"); // Handle empty data case
            }

            // Loop through each medication and save it to the database
            foreach (var medication in medications)
            {
                var adminMedication = new AdministerMedication
                {
                    AdmittedPatientID = admittedPatientId,
                    PrescriptionID = prescriptionID,
                    AccountID = accountID,
                    MedicationID = medication.MedicationID,
                    AdministerQuantity = medication.AdministeredQuantity,
                    Time = TimeOnly.FromDateTime(DateTime.Now), // Capture the current time
                    Date = DateOnly.FromDateTime(DateTime.Now)  // Capture the current date
                };

                _dbContext.AdministerMedication.Add(adminMedication);
            }

            // Commit the changes to the database
            _dbContext.SaveChanges();
            // Get the total administered quantity for the given prescription
            var totalAdministeredQuantity = _dbContext.AdministerMedication
                .Where(am => am.PrescriptionID == prescriptionID)
                .GroupBy(am => am.MedicationID)
                .Select(g => new
                {
                    MedicationID = g.Key,
                    TotalQuantity = g.Sum(am => am.AdministerQuantity)
                })
                .ToList();

            // Check if the total administered quantity matches the required quantity from MedicationInstructions
            var medicationInstructions = _dbContext.MedicationInstructions
                .Where(m => m.PrescriptionID == prescriptionID)
                .ToList();

            bool allMedicationsAdministered = true;

            foreach (var instruction in medicationInstructions)
            {
                var administeredMedication = totalAdministeredQuantity
                    .FirstOrDefault(am => am.MedicationID == instruction.MedicationID);

                if (administeredMedication == null || administeredMedication.TotalQuantity < instruction.Quantity)
                {
                    allMedicationsAdministered = false;
                    break;
                }
            }

            // If all medications have been administered, update the prescription status to "Administered"
            if (allMedicationsAdministered)
            {
                var prescription = _dbContext.Prescription
                    .FirstOrDefault(p => p.PrescriptionID == prescriptionID);

                if (prescription != null)
                {
                    prescription.Status = "Administered";
                    _dbContext.SaveChanges(); // Save the updated prescription status
                }
            }

            return RedirectToAction("MedicationCollected", new
            {
                prescriptionID,
                name,
                surname,
                ward,
                bed,
                admittedPatientId,
                accountID
            });
        }


        public IActionResult PatientRecords(int AdmittedPatientID, string name, string surname, int accountID)
        {
            var comboData = (from a in _dbContext.Accounts
                                join b in _dbContext.BookSurgery on a.AccountID equals b.AccountID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join ap in _dbContext.AdmittedPatients on p.PatientID equals ap.PatientID
                                join bed in _dbContext.Bed on ap.BedId equals bed.BedId
                                join w in _dbContext.Ward on bed.WardID equals w.WardId
                                join status in _dbContext.AdmissionStatus on ap.AdmissionStatusID equals status.AdmissionStatusId
                                where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID

                                select new BookedPatientInfo
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
            var viewModel = new BookedPatientInfo
            {
                AllcombinedData = comboData,

            };
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);


            ViewBag.AccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;

            return View(viewModel);
        }
        public IActionResult PatientVitals(int AdmittedPatientID, string name, string surname, int accountID)
        {
            var patientVitals = (from pv in _dbContext.PatientVitals
                                 join ap in _dbContext.AdmittedPatients
                                 on pv.PatientID equals ap.PatientID
                                 where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID

                                 select new PatientAllergyViewModel
                                 {
                                     Date = ap.Date,
                                     Time = pv.time,
                                     Height = ap.Height,
                                     Weight = ap.Weight,
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
                           where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID
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
                              where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID
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
                              where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID
                              select new PatientAllergyViewModel
                              {
                                  Name = pi.Name,
                                  Surname = pi.Surname,
                                  MedicationName = cm.MedicationName // Ensure this property exists in your view model
                              }).OrderBy(cm => cm.MedicationName).ToList();
            var administerMeds = (from ad in _dbContext.AdministerMedication
                                  join ap in _dbContext.AdmittedPatients on ad.AdmittedPatientID equals ap.AdmittedPatientID
                                  join pt in _dbContext.PatientInfo on ap.PatientID equals pt.PatientID
                                  join cm in _dbContext.Medication on ad.MedicationID equals cm.MedicationID
                                  where ap.AdmittedPatientID == AdmittedPatientID && ap.AccountID == accountID
                                  select new PatientAllergyViewModel
                                  {
                                      MedicationName = cm.MedicationName,
                                      Quantity = ad.AdministerQuantity
                                      
                                  }).OrderBy(cm => cm.MedicationName).ToList();

            // Create a view model that holds both lists
            var viewModel = new PatientAllergyViewModel
            {
                AllAdministeredMedication = administerMeds,
                Allvitals = patientVitals,
                Allallergy = allergy,
                AllConditions = conditions,
                AllCurrentMed = currentMed
            };
            ViewBag.AccountID = accountID;
            ViewBag.PatientName = name;
            ViewBag.PatientSurname = surname;

            return View(viewModel);







        }

        public IActionResult InfoNurse()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Vitals(int admittedPatientID, string name, string Surname, string Ward, int Bed, int accountID)
        {
            // Fetch the admitted patient details
            var admittedPatient = _dbContext.AdmittedPatients
                .FirstOrDefault(ad => ad.AdmittedPatientID == admittedPatientID);

            if (admittedPatient == null)
            {
                return NotFound("Patient not found.");
            }

            // Create a ViewModel to pass to the view
            var viewModel = new ViewVitals
            {
                AdmittedPatientID = admittedPatientID,
                PatientID = admittedPatient.PatientID // Get PatientID from AdmittedPatientsModel
            };
            ViewBag.AccountID = accountID;
            ViewBag.PatientSurname = Surname;
            ViewBag.PatientName = name;
            ViewBag.patientWard = Ward;
            ViewBag.patientBed = Bed;
            ViewBag.PHeight = admittedPatient.Height;
            ViewBag.PWeight = admittedPatient.Weight;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Vitals(ViewVitals model, int accountID, string notes)
        {
            if (ModelState.IsValid)
            {
                var admittedPatient = _dbContext.AdmittedPatients
                    .FirstOrDefault(ad => ad.AdmittedPatientID == model.AdmittedPatientID);

                if (admittedPatient == null)
                {
                    TempData["ErrorMessage"] = "Admitted patient not found.";
                    ViewBag.AccountID = accountID;
                    return View(model);
                }

                // Create a new PatientVitals object
                var patientVitals = new PatientVitals
                {
                    PatientID = admittedPatient.PatientID,
                    SystolicBloodPressure = model.SystolicBloodPressure,
                    DiastolicBloodPressure = model.DiastolicBloodPressure,
                    HeartRate = model.HeartRate,
                    BloodOxygen = model.BloodOxygen,
                    Respiration = model.Respiration,
                    BloodGlucoseLevel = model.BloodGlucoseLevel,
                    Temperature = model.Temperature,
                    time = TimeOnly.FromDateTime(DateTime.Now)
                };

                _dbContext.PatientVitals.Add(patientVitals);
                _dbContext.SaveChanges();

                // Redirect based on notes
                if (!string.IsNullOrWhiteSpace(notes))
                {
                    return RedirectToAction("EmailVitals", "Nurse", new { AdmittedPatientID = model.AdmittedPatientID, accountID, notes = notes });
                }

                return RedirectToAction("AdmittedPatients", "Nurse", new { accountID });
            }

            ViewBag.AccountID = accountID;
            return View(model);
        }


        public async Task<IActionResult> EmailVitals(int AdmittedPatientID, string notes)
        {
            var combinedData = await (from pv in _dbContext.PatientVitals
                                      join ad in _dbContext.AdmittedPatients on pv.PatientID equals ad.PatientID
                                      join b in _dbContext.BookSurgery on ad.BookingID equals b.BookingID
                                      join ac in _dbContext.Accounts on b.AccountID equals ac.AccountID
                                      join p in _dbContext.PatientInfo on ad.PatientID equals p.PatientID
                                      join w in _dbContext.Ward on ad.WardID equals w.WardId
                                      join bed in _dbContext.Bed on ad.BedId equals bed.BedId
                                      where ad.AdmittedPatientID == AdmittedPatientID && pv.PatientID == ad.PatientID
                                      orderby ad.Date descending, pv.time descending
                                      select new EmailVital
                                      {
                                          AdmittedPatientID = ad.AdmittedPatientID,
                                          Height = ad.Height,
                                          Weight = ad.Weight,
                                          SystolicBloodPressure = pv.SystolicBloodPressure,
                                          DiastolicBloodPressure = pv.DiastolicBloodPressure,
                                          HeartRate = pv.HeartRate,
                                          BloodOxygen = pv.BloodOxygen,
                                          Respiration = pv.Respiration,
                                          BloodGlucoseLevel = pv.BloodGlucoseLevel,
                                          Temperature = pv.Temperature,
                                          Time = pv.time,
                                          FullName = p.Name + " " + p.Surname,
                                          WardName = w.WardName,
                                          Bed = bed.Number,
                                          SurgeonEmail = ac.Email,
                                          SurgeonRole = ac.Role
                                      }).FirstOrDefaultAsync();

            if (combinedData == null)
            {
                TempData["ErrorMessage"] = "Vitals or patient not found.";
                return RedirectToAction("AdmittedPatients", "Nurse");
            }

            // Split and number the notes
            var formattedNotes = string.Empty;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                var splitNotes = notes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                formattedNotes = string.Join(Environment.NewLine, splitNotes.Select((note, index) => $"{index + 1}. {note.Trim()}"));
            }

            // Prepare the view model
            var viewModel = new EmailVital
            {
                AdmittedPatientID = combinedData.AdmittedPatientID,
                Height = combinedData.Height,
                Weight = combinedData.Weight,
                SystolicBloodPressure = combinedData.SystolicBloodPressure,
                DiastolicBloodPressure = combinedData.DiastolicBloodPressure,
                HeartRate = combinedData.HeartRate,
                BloodOxygen = combinedData.BloodOxygen,
                Respiration = combinedData.Respiration,
                BloodGlucoseLevel = combinedData.BloodGlucoseLevel,
                Temperature = combinedData.Temperature,
                Time = combinedData.Time,
                FullName = combinedData.FullName,
                WardName = combinedData.WardName,
                Bed = combinedData.Bed,
                SurgeonEmail = combinedData.SurgeonEmail,
                SurgeonRole = combinedData.SurgeonRole,
                Notes = formattedNotes // Assign the formatted notes
            };

            // Pass the account ID via ViewBag (if needed)
            ViewBag.AccountID = HttpContext.Session.GetInt32("AccountID");

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EmailVitals(string notes, int AdmittedPatientID,int accountID)
        {
            var nurseEmail = HttpContext.Session.GetString("UserEmail");

            if (nurseEmail == null)
            {
                TempData["ErrorMessage"] = "Could not retrieve the nurse's email.";
                return RedirectToAction("AdmittedPatients", "Nurse");
            }

            // Fetch surgeon's email and patient information again
            var combinedData = await (from pv in _dbContext.PatientVitals
                                      join ad in _dbContext.AdmittedPatients on pv.PatientID equals ad.PatientID
                                      join b in _dbContext.BookSurgery on ad.BookingID equals b.BookingID
                                      join ac in _dbContext.Accounts on b.AccountID equals ac.AccountID
                                          join p in _dbContext.PatientInfo on ad.PatientID equals p.PatientID
                                          join w in _dbContext.Ward on ad.WardID equals w.WardId
                                          join bed in _dbContext.Bed on ad.BedId equals bed.BedId
                                          where ad.AdmittedPatientID == AdmittedPatientID
                                          orderby ad.Date descending, pv.time descending // Get the latest vitals
                                          select new EmailVital
                                          {
                                              AdmittedPatientID = AdmittedPatientID,
                                              Height = ad.Height,
                                              Weight = ad.Weight,
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
                                              
                                          }).FirstOrDefaultAsync(); // Get only the latest record

          
            // Prepare the email
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
                    await client.AuthenticateAsync("jansen.ronaldocullen@gmail.com", "xqqx kiox hcgm xvmr");
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



        public IActionResult NurseReport()
        {
            var accountIDString = HttpContext.Session.GetString("UserAccountId");
            if (!int.TryParse(accountIDString, out int accountID))
            {
                // Handle the case where accountID is not available or is invalid


                accountID = 0; // Or handle as required
            }

            var combinedData = (from p in _dbContext.PatientInfo
                                join bs in _dbContext.BookSurgery on p.PatientID equals bs.PatientID
                                join ap in _dbContext.AdmittedPatients on bs.PatientID equals ap.PatientID
                                join ac in _dbContext.Accounts on ap.AccountID equals ac.AccountID
                                join dp in _dbContext.DispensedScriptsModel on ac.AccountID equals dp.AccountID
                                join pr in _dbContext.Prescription on dp.PrescriptionID equals pr.PrescriptionID
                                join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                join ma in _dbContext.PharmacyMedication on dp.PrescriptionID equals ma.MedicationID
                                join am in _dbContext.AdministerMedication on mi.MedicationID equals am.MedicationID
                                join m in _dbContext.Medication on ma.MedicationID equals m.MedicationID
                                where ap.AccountID == accountID && pr.Status == "Administered"
                                orderby p.Name
                                select new NurseReportViewModel
                                {
                                    Date = am.Date,
                                    MedicationName = m.MedicationName,
                                    Quantity = am.AdministerQuantity,
                                    Patient = p.Name + " " + p.Surname,

                                    Time = am.Time,

                                }).ToList();
            var administerMeds = (from ad in _dbContext.AdministerMedication
                                  join ap in _dbContext.AdmittedPatients on ad.AdmittedPatientID equals ap.AdmittedPatientID
                                  join pt in _dbContext.PatientInfo on ap.PatientID equals pt.PatientID
                                  join cm in _dbContext.Medication on ad.MedicationID equals cm.MedicationID
                                  where ap.AccountID == accountID
                                  select new NurseReportViewModel
                                  {
                                      MedicationName = cm.MedicationName,
                                      Quantity = ad.AdministerQuantity,
                                      Patient = pt.Name + " " + pt.Surname,
                                      Date = ad.Date,
                                      Time = ad.Time,

                                  }).OrderBy(cm => cm.MedicationName).ToList();
            // Summary Data Calculation
            var medicationSummary = (from p in _dbContext.PatientInfo
                                     join bs in _dbContext.BookSurgery on p.PatientID equals bs.PatientID
                                     join ap in _dbContext.AdmittedPatients on bs.PatientID equals ap.PatientID
                                     join ac in _dbContext.Accounts on ap.AccountID equals ac.AccountID
                                     join dp in _dbContext.DispensedScriptsModel on ac.AccountID equals dp.AccountID
                                     join pr in _dbContext.Prescription on dp.PrescriptionID equals pr.PrescriptionID
                                     join mi in _dbContext.MedicationInstructions on pr.PrescriptionID equals mi.PrescriptionID
                                     join ma in _dbContext.PharmacyMedication on dp.PrescriptionID equals ma.MedicationID
                                     join am in _dbContext.AdministerMedication on mi.MedicationID equals am.MedicationID
                                     join m in _dbContext.Medication on ma.MedicationID equals m.MedicationID
                                     where ap.AccountID == accountID && pr.Status == "Administered"
                                     group am by m.MedicationName into g
                                     select new
                                     {
                                         MedicationName = g.Key,
                                         TotalQuantity = g.Sum(x => x.AdministerQuantity)
                                     }).ToList();
           

            var viewModel = new NurseReportViewModel
            {
                AllcombinedData = administerMeds,

            };

            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            var AdministeredMedicationCount = _dbContext.AdministerMedication
                .Where(bs => bs.AccountID == accountID && bs.Date == today)
                .Count();




            // Pass the prescribed count to the view
            ViewBag.AdministeredMedicationCount = AdministeredMedicationCount;
            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.AccountID = accountID;
            return View(viewModel);
        }




       
    }
}
