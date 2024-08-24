using DEMO.Data;
using DEMO.Models;
using DEMO.Models.NurseModels;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;

namespace DEMO.Controllers
{
    public class NurseController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NurseController(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult MainPage()
        {
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
                                    PatinetSurname = p.Surname,
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
        }
        
        public IActionResult AdmissionPage()
        {
            return View();
        }
        public IActionResult AdmittedPatients()
        {
            return View();
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
        public IActionResult InfoNurse()
        {
            return View();
        }
        public IActionResult EmailVitals()
        {
            return View();
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
//            var viewModel = new EmailViewModel
//            {
//                AccountID = user.AccountID,
//                FullName = $"{user.Name} {user.Surname}",
//                Email = user.Email,
//                Username = user.Username,
//                Password = user.Password, // Only for example; usually, passwords are not shared like this
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
//            emailMessage.From.Add(new MailboxAddress("Day Hospital-  Apollo+", "noreply@dayhospital.com"));
//            emailMessage.To.Add(new MailboxAddress(user.Role, user.Email));
//            emailMessage.Subject = "User Added";

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
