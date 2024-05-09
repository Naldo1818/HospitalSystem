using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class PharmacistController : Controller
    {
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


        public IActionResult ViewAllActivePrescriptionsPage()
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



		public ActionResult PrescriptionApproved()
		{
			ViewBag.Message = "This is a pop-up message.";
			return View();
		}
	}
}
