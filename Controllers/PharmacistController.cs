using DEMO.Data;
using DEMO.Data.Migrations;
using DEMO.Models;
using DEMO.ViewModels;
using DEMO.Models.PharmacistModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics.Contracts;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore.Internal;

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






        public IActionResult ViewAllActivePrescriptionsPage()

        {

            var combinedData = (from prescription in _dbContext.Prescription
                                join medicationInstruction in _dbContext.MedicationInstructions
                                on prescription.PrescriptionID equals medicationInstruction.PrescriptionID
                                join medication in _dbContext.Medication
                                on medicationInstruction.MedicationID equals medication.MedicationID
                                join patient in _dbContext.PatientInfo
                                on prescription.AccountID equals patient.PatientID // Assuming AccountID is linked to PatientID
                                join account in _dbContext.Accounts
                                on prescription.AccountID equals account.AccountID
                                where prescription.Status=="Prescribed" 
                                select new ViewActivePrescriptionsModel


                                {
                                    // Prescription fields
                                    SurgeonName= account.Name,
                                    SurgeonSurname=account.Surname,
                                    DateGiven = prescription.DateGiven,
                                    Urgency = prescription.Urgency,
                                    Take = prescription.Take,
                                    Status = prescription.Status,

                                    // Medication fields


                                    // MedicationInstructions fields


                                    // PatientInfo fields
                                    Name = patient.Name,
                                    Surname = patient.Surname,



                                }).ToList();

            var viewModel = new ViewActivePrescriptionsModel
            {
                combinedData = combinedData,
            };

            return View(viewModel);


           
        }








    }
}













    

