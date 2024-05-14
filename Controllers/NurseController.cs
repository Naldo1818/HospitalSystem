using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DEMO.Controllers
{
    public class NurseController : Controller
    {
        public IActionResult MainPage()
        {
            return View();
        }
        public IActionResult Vitals()
        {
            return View();

        }


    }
}
