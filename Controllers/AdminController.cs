using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
