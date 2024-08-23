using DEMO.Data;
using DEMO.Data.Migrations;
using DEMO.Models;
using DEMO.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

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

        public async Task<IActionResult> ViewAllActivePrescriptionsPage()

        {
            var activePrescriptions = await _dbContext.Prescription
            
            .ToListAsync();

            return View(activePrescriptions);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PharmacistHomePage()
        {
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

        public IActionResult ViewSpecificPrescription()
        {
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

        public ActionResult AddMedication()
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


    }
}
