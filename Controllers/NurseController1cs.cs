using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class NurseController1cs : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
