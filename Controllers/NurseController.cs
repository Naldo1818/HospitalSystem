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

        [HttpGet]
        public IActionResult Vitals(int admittedPatientID, string name, string Surname, string Ward, int Bed)
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

            ViewBag.PatientSurname = Surname;
            ViewBag.PatientName = name;
            ViewBag.patientWard = Ward;
            ViewBag.patientBed = Bed;
            ViewBag.PHeight = admittedPatient.Height;
            ViewBag.PWeight = admittedPatient.Weight;

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Vitals(ViewVitals model)
        {

            if (ModelState.IsValid)
            {
                var admittedPatient = _dbContext.AdmittedPatients
                .FirstOrDefault(ad => ad.AdmittedPatientID == model.AdmittedPatientID);
                // Create a new PatientVitals object
                var patientVitals = new PatientVitals
                {
                    PatientID = admittedPatient.PatientID, // Use the PatientID from the ViewModel
                    SystolicBloodPressure = model.SystolicBloodPressure,
                    DiastolicBloodPressure = model.DiastolicBloodPressure,
                    HeartRate = model.HeartRate,
                    BloodOxygen = model.BloodOxygen,
                    Respiration = model.Respiration,
                    BloodGlucoseLevel = model.BloodGlucoseLevel,
                    Temperature = model.Temperature,
                    time = TimeOnly.FromDateTime(DateTime.Now) // Or use another property if needed
                };

                // Add the new vitals entry to the database
                _dbContext.PatientVitals.Add(patientVitals);
                _dbContext.SaveChanges();

                // Redirect to another action, e.g., the list of admitted patients
                return RedirectToAction("AdmittedPatients", "Nurse"); // Adjust as necessary
            }
            
            return View(model);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Discharge(AdmittedPatientsModel model)
        {
            if (ModelState.IsValid)
            {
                var admittedPatient = _dbContext.AdmittedPatients.FirstOrDefault(ap => ap.AdmittedPatientID == model.AdmittedPatientID);

                if (admittedPatient != null)
                {
                    // Update the existing account with new values
                    admittedPatient.AdmissionStatusID = 2;
                    _dbContext.SaveChanges();
                }

                return RedirectToAction("patientAddmision");
            }

            return View(model);
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
                                where ap.AccountID == accountID

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


            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;

            //var patients = _dbContext.AdmittedPatients.ToList();
            return View("AdmittedPatients", viewModel);
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
        public IActionResult MedicationCollection(string name, string surname, string ward, int bed, int admittedPatientId)
        {
            // Fetch the dispensed prescriptions along with patient info
            var combinedData = (from p in _dbContext.PatientInfo
                                       join ap in _dbContext.AdmittedPatients
                                       on p.PatientID equals ap.PatientID
                                       join pr in _dbContext.Prescription
                                       on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                       join rs in _dbContext.DispensedScriptsModel
                                       on pr.PrescriptionID equals rs.PrescriptionID
                                       join a in _dbContext.Accounts
                                       on rs.AccountID equals a.AccountID
                                       where pr.Status == "Dispensed" 
                                       orderby pr.Urgency == "Yes" descending, p.Name
                                       select new PrescriptionListViewModal
                                       {
                                           PrescriptionID = pr.PrescriptionID,
                                           PatientName = p.Name,
                                           PatientSurname = p.Surname,
                                           DateGiven = pr.DateGiven,
                                           Urgency = pr.Urgency,
                                           Status = pr.Status,
                                           AccountName = a.Name,
                                           AccountSurname = a.Surname
                                       }).ToList();

            // Retrieve user information from the session
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");

            // Populate ViewBag with necessary information
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.patientWard = ward;
            ViewBag.patientBed = bed;
            ViewBag.time = TimeOnly.FromDateTime(DateTime.Now);

            // Create a ViewModel instance and pass the data to the view
            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribedDispensed = combinedData // Populate dispensed prescriptions
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult CollectMedication(int prescriptionId)
        {
            // Find the prescription in the database
            var prescription = _dbContext.Prescription.FirstOrDefault(p => p.PrescriptionID == prescriptionId);

            if (prescription == null)
            {
                // Handle the case where the prescription is not found (optional)
                return NotFound("Prescription not found.");
            }

            // Update the status to "collected"
            prescription.Status = "Collected";

            // Save changes to the database
            _dbContext.SaveChanges();

            // Optionally, return a success message or redirect to another action
            return RedirectToAction("MedicationCollection", new { /* parameters as needed */ });
        }
        public IActionResult MedicationAdministration(string name, string surname, string ward, int bed, int admittedPatientId, DateOnly date)
        {
            // Fetch the collected prescriptions along with patient info
            var combinedData = (from p in _dbContext.Prescription
                                join ap in _dbContext.AdmittedPatients
                                on p.AdmittedPatientID equals ap.AdmittedPatientID
                                join pa in _dbContext.PatientInfo
                                on ap.PatientID equals pa.PatientID
                                join ac in _dbContext.Accounts on p.AccountID equals ac.AccountID
                                where p.AdmittedPatientID == admittedPatientId && p.Status == "Collected"
                                select new PrescriptionListViewModal
                                {
                                    PatientName = pa.Name, // Adjust as per your model
                                    PatientSurname = pa.Surname, // Adjust as per your model
                                    DateGiven = p.DateGiven,
                                    AccountName = ac.Name, // Adjust as per your model
                                    AccountSurname = ac.Surname, // Adjust as per your model
                                    Urgency = p.Urgency,
                                    Status = p.Status,
                                    PrescriptionID = p.PrescriptionID
                                }).ToList(); // Convert to list to execute the query

            // Retrieve user information from the session
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Populate ViewBag with necessary information
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;
            ViewBag.patientWard = ward;
            ViewBag.patientBed = bed;
            ViewBag.date = date;

            // Create a ViewModel instance and pass the data to the view
            var viewModel = new PrescriptionListViewModal
            {
                AllPrescribedDispensed = combinedData // Populate collected prescriptions
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AdministerMedication(List<AdministeredMedicationViewModel> administeredMedications, int admittedPatientId)
        {
            // Iterate through each administered medication
            foreach (var medication in administeredMedications)
            {
                // Fetch the prescription for validation
                var prescription = _dbContext.Prescription
                    .FirstOrDefault(p => p.PrescriptionID == medication.PrescriptionID && p.AdmittedPatientID == admittedPatientId);
                MedicationInstructions medinstruction = new MedicationInstructions
                {
                    PrescriptionID = medication.PrescriptionID,
                };
                Medication meds = new Medication { };
                if (prescription != null)
                {
                    // Calculate the total administered quantity for the medication
                    var totalAdministered = _dbContext.AdministerMedication
                        .Where(am => am.PrescriptionID == medication.PrescriptionID)
                        .Sum(am => am.AdministerQuantity) + medication.QuantityAdministered;

                    // Check if the total administered exceeds the prescribed quantity
                    if (totalAdministered > medinstruction.Quantity)
                    {
                        ModelState.AddModelError("Quantity", $"Total administered quantity for {meds.MedicationName} cannot exceed the prescribed quantity of {medinstruction.Quantity}.");
                        return View(); // Return with errors
                    }

                    // Create an entry for the administered medication
                    var administeredMedication = new AdministerMedication
                    {
                        PrescriptionID = medication.PrescriptionID,
                        AdministerQuantity = medication.QuantityAdministered,
                        Date = DateOnly.FromDateTime(DateTime.Now),                         
                        Time = TimeOnly.FromDateTime(DateTime.Now)
                    };

                    // Save to the database
                    _dbContext.AdministerMedication.Add(administeredMedication);
                }
            }

            // Save changes to the database
            _dbContext.SaveChanges();

            return RedirectToAction("MedicationAdministration", new { admittedPatientId }); // Redirect as needed
        }
        public IActionResult PatientRecords(int AdmittedPatientID, string name, string surname)
        {
            var comboData = (from a in _dbContext.Accounts
                                join b in _dbContext.BookSurgery on a.AccountID equals b.AccountID
                                join p in _dbContext.PatientInfo on b.PatientID equals p.PatientID
                                join ap in _dbContext.AdmittedPatients on p.PatientID equals ap.PatientID
                                join bed in _dbContext.Bed on ap.BedId equals bed.BedId
                                join w in _dbContext.Ward on bed.WardID equals w.WardId
                                join status in _dbContext.AdmissionStatus on ap.AdmissionStatusID equals status.AdmissionStatusId
                                where ap.AdmittedPatientID == AdmittedPatientID

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



            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            ViewBag.PatientSurname = surname;
            ViewBag.PatientName = name;

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



        public async Task<IActionResult> EmailVitals(int AdmittedPatientID)
        {
            var combinedData = await (from pv in _dbContext.PatientVitals
                                      join ad in _dbContext.AdmittedPatients on pv.PatientID equals ad.PatientID
                                      join b in _dbContext.BookSurgery on ad.BookingID equals b.BookingID
                                      join ac in _dbContext.Accounts on b.AccountID equals ac.AccountID
                                      join p in _dbContext.PatientInfo on ad.PatientID equals p.PatientID
                                      join w in _dbContext.Ward on ad.WardID equals w.WardId
                                      join bed in _dbContext.Bed on ad.BedId equals bed.BedId
                                      where ad.AdmittedPatientID == AdmittedPatientID && pv.PatientID == ad.PatientID
                                      orderby pv.time descending, ad.Date descending
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
                SurgeonRole = combinedData.SurgeonRole
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EmailVitals(string notes, int AdmittedPatientID)
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
                                          orderby pv.time descending, ad.Date descending // Get the latest vitals
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
                                where ap.AccountID == accountID && pr.Status == "Complete"
                                orderby p.Name
                                select new NurseReportViewModel
                                {
                                    MedicationName = m.MedicationName,
                                    Quantity = mi.Quantity,
                                    PatientName = p.Name,
                                    PatientSurname = p.Surname,
                                    AdministeredQuantity = am.AdministerQuantity,
                                    Date = am.Date,
                                    Time = TimeOnly.FromDateTime(DateTime.Now),

                                }).ToList();

            var viewModel = new NurseReportViewModel
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


            var prescribedCount = (from p in _dbContext.PatientInfo
                                   join ap in _dbContext.AdmittedPatients
                                   on p.PatientID equals ap.PatientID
                                   join pr in _dbContext.Prescription
                                   on ap.AdmittedPatientID equals pr.AdmittedPatientID
                                   where pr.Status == "Prescribed" && pr.AccountID == accountID
                                   select pr).Count();

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
            ViewBag.PrescribedCount = prescribedCount;
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
