using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
