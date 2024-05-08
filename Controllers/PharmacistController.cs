using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
