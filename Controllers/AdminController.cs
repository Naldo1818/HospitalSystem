﻿using Microsoft.AspNetCore.Mvc;

namespace DEMO.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminHome()
        {
            return View();
        }
        public IActionResult AddMedication()
        {
            return View();
        }
        public IActionResult AddActiveingredient()
        {
            return View();
        }
    }
}
