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

    }
}
