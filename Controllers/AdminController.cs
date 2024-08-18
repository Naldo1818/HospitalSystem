using DEMO.Data;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DEMO.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult AdminHome(int accountId)
        {// Try to get data from session first
            var accountID = HttpContext.Session.GetString("UserAccountId");
            var name = HttpContext.Session.GetString("UserName");
            var surname = HttpContext.Session.GetString("UserSurname");
            var email = HttpContext.Session.GetString("UserEmail");

            if (!string.IsNullOrEmpty(accountID) && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(surname) && !string.IsNullOrEmpty(email))
            {
                // Use existing session data
                ViewBag.UserName = accountID;
                ViewBag.UserName = name;
                ViewBag.UserSurname = surname;
                ViewBag.UserEmail = email;
            }
            else
            {
                // Retrieve from database if not in session
                var admin = _dbContext.Accounts
                    .Where(a => a.AccountID == accountId)
                    .Select(a => new SurgeonViewModel
                    {
                        AccountID = a.AccountID,
                        Name = a.Name,
                        Surname = a.Surname,
                        Email = a.Email
                    })
                    .SingleOrDefault();

                if (admin == null)
                {
                    return NotFound();
                }

                // Store user data in session
                HttpContext.Session.SetString("UserAccountId", admin.AccountID.ToString());
                HttpContext.Session.SetString("UserName", admin.Name);
                HttpContext.Session.SetString("UserSurname", admin.Surname);
                HttpContext.Session.SetString("UserEmail", admin.Email);

                ViewBag.UserAccountID = admin.AccountID.ToString();
                ViewBag.UserName = admin.Name;
                ViewBag.UserSurname = admin.Surname;
                ViewBag.UserEmail = admin.Email;
            }
            return View();
        }
       
        public IActionResult ListUser()
        {
            var allAccounts = _dbContext.Accounts.ToList();
            var viewModel = new AccountsListViewModal
            {
                AllAccounts = allAccounts,

            };

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");


            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddUser(Accounts model)
        {

            if (ModelState.IsValid)
            {
                Accounts newAccount = new Accounts
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Username = model.Username,
                    Password = model.Password,
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    Role = model.Role,
                    Status = model.Status

                };

                _dbContext.Accounts.Add(newAccount);
                _dbContext.SaveChanges();

                return RedirectToAction("ListUser");
            }

            // If validation fails, redisplay the form with errors
            return View("ListUser", model);
        }
        [HttpPost]
        public IActionResult EditUser(Accounts model)
        {
            if (ModelState.IsValid)
            {
                var accountUser = _dbContext.Accounts.FirstOrDefault(p => p.AccountID == model.AccountID);

                if (accountUser != null)
                {
                    // Update the existing account with new values
                    accountUser.Name = model.Name;
                    accountUser.Surname = model.Surname;
                    accountUser.Username = model.Username;
                    accountUser.Password = model.Password;
                    accountUser.ContactNumber = model.ContactNumber;
                    accountUser.Email = model.Email;
                    accountUser.Role = model.Role;
                    accountUser.Status = model.Status;

                    _dbContext.SaveChanges();
                }

                return RedirectToAction("ListUser");
            }

            // Return to the view with the model in case of validation errors
            return View(model);
        }

        public IActionResult ListMedication()
        {
            // Fetch the combined data
            var medicationActiveIngredients = (from ma in _dbContext.MedicationActiveIngredient
                                               join m in _dbContext.Medication on ma.MedicationID equals m.MedicationID
                                               join a in _dbContext.Activeingredient on ma.ActiveingredientID equals a.ActiveingredientID
                                               select new MedicationListViewModel
                                               {
                                                   MedicationName = m.MedicationName,
                                                   ActiveIngredientName = a.ActiveIngredientName,
                                                   ActiveIngredientStrength = ma.ActiveIngredientStrength
                                               }).ToList();

            // Fetch additional lists if needed
            var allMedication = _dbContext.Medication.ToList();
            var activeIngredients = _dbContext.Activeingredient.ToList();

            // Create the view model
            var viewModel = new MedicationListViewModel
            {
                AllMedicationActiveIngredients = medicationActiveIngredients,
                AllMedication = allMedication,
                ActiveIngredients = activeIngredients
            };

            // Set ViewBag properties
            ViewBag.UserAccountID = HttpContext.Session.GetString("UserAccountId");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
            ViewBag.UserSurname = HttpContext.Session.GetString("UserSurname");
            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddMedication(Medication model)
        {
            if (ModelState.IsValid)
            {
                Medication newMedication = new Medication
                {
                    MedicationName = model.MedicationName,
                    MedicationForm = model.MedicationForm,
                    Schedule = model.Schedule

                };

                _dbContext.Medication.Add(newMedication);
                _dbContext.SaveChanges();

                return RedirectToAction("ListMedication");
            }

            // If validation fails, redisplay the form with errors
            return View("ListMedication", model);
        }
        [HttpPost]
        public IActionResult AddActiveingredient(MedicationActiveIngredient model)
        {
            if (ModelState.IsValid)
            {
                MedicationActiveIngredient newMedicationActiveIngredient = new MedicationActiveIngredient
                {
                    MedicationID = model.MedicationID,
                    ActiveingredientID = model.ActiveingredientID,
                    ActiveIngredientStrength = model.ActiveIngredientStrength

                };

                _dbContext.MedicationActiveIngredient.Add(newMedicationActiveIngredient);
                _dbContext.SaveChanges();

                return RedirectToAction("ListMedication");
            }

            // If validation fails, redisplay the form with errors
            return View("ListMedication", model);
        }

        public IActionResult ListTreatmentCodes()
        {
            var allTreatmentCodes = _dbContext.TreatmentCodes.ToList();
            var viewModel = new TreatmentCodesListViewModal
            {
                AllTreatmentCodes = allTreatmentCodes,

            };

            var accountID = HttpContext.Session.GetString("UserAccountId");
            var userName = HttpContext.Session.GetString("UserName");
            var userSurname = HttpContext.Session.GetString("UserSurname");
            var userEmail = HttpContext.Session.GetString("UserEmail");

            ViewBag.UserAccountID = accountID;
            ViewBag.UserName = userName;
            ViewBag.UserSurname = userSurname;
            ViewBag.UserEmail = userEmail;
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult AddTreatmentCode(TreatmentCodes model)
        {

            if (ModelState.IsValid)
            {
                TreatmentCodes newTreatmentCodes = new TreatmentCodes
                {
                    TreatmentName = model.TreatmentName,
                    TreatmentCode = model.TreatmentCode,
                   
                };

                _dbContext.TreatmentCodes.Add(newTreatmentCodes);
                _dbContext.SaveChanges();

                return RedirectToAction("ListTreatmentCodes");
            }

            // If validation fails, redisplay the form with errors
            return View("ListTreatmentCodes", model);
        }
        [HttpPost]
        public IActionResult EditTreatmentCodes(TreatmentCodes model)
        {
            if (ModelState.IsValid)
            {
                var treatment = _dbContext.TreatmentCodes.FirstOrDefault(p => p.TreatmentCodeID == model.TreatmentCodeID);

                if (treatment != null)
                {
                    // Update the existing account with new values
                    treatment.TreatmentName = model.TreatmentName;
                    treatment.TreatmentCode = model.TreatmentCode;
                   

                    _dbContext.SaveChanges();
                }

                return RedirectToAction("ListTreatmentCodes");
            }

            // Return to the view with the model in case of validation errors
            return View(model);
        }
    }
}
