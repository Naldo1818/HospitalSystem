using DEMO.Data;
using DEMO.Models;
using DEMO.Models.NurseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DEMO.Controllers
{
    public class LookupController : Controller
    {

        private readonly ApplicationDbContext _dbContext;

        public LookupController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Province,City,Suburb
        public List<Province> GetAllProvinces()
        {
            var city = new List<Province>();
            city =  _dbContext.Provinces.ToList();
            return city;
        }

        public List<City> GetCitiesByProvinceId(int ProvinceID)
        {
            var city = new List<City>() ;
            if (ProvinceID > 0)
            {
                city =  _dbContext.City.Where(x => x.ProvinceID == ProvinceID).ToList();
            }
            else
            {
                city = _dbContext.City.ToList(); //exception or show all cities?
            }

            return city;
        }

        public List<Suburb> GetSuburbsByCityId(int CityID)
        {
            var suburb = new List<Suburb>();
            if (CityID > 0)
            {
                suburb = _dbContext.Suburb.Where(x => x.CityID == CityID).ToList();
            }
            else
            {
                suburb = _dbContext.Suburb.ToList(); //exception or show all suburbs?
            }
            return suburb;
        }

        public int GetPostalCodeBySuburbId(int SuburbId)
        {
            var postalCode = 0;
            if (SuburbId > 0)
            {
                postalCode = _dbContext.Suburb.Where(x => x.SuburbID == SuburbId).FirstOrDefault().PostalCode;
            }
            else
            {
                postalCode = 0; //exception or show all suburbs?
            }
            return postalCode;
        }

        #endregion

        #region Ward,Bed
        public List<Ward> GetAllWards()
        {
            return _dbContext.Ward.ToList(); 
        }

        public List<Bed> GetBedsByWardId(int WardID)
        {
            if (WardID > 0)
            {
                return _dbContext.Bed.Where(x => x.WardID == WardID).ToList();
            }
            else
            {
                return _dbContext.Bed.ToList(); //exception or show all beds?
            }
        }

        #endregion

        #region Condition, Allergies, Medication
        public List<Condition> GetAllConditions()
        {
            var condition = new List<Condition>();
            condition = _dbContext.Condition.ToList();
            return condition;
        }

        public List<Activeingredient> GetAllAllergies()
        {
            var allergies = new List<Activeingredient>();
            allergies =  _dbContext.Activeingredient.ToList();
            return allergies;
        }

        public List<Medication> GetAllMedications()
        {
            var medication = new List<Medication>();
            medication = _dbContext.Medication.ToList();
            return medication;
        }

        public List<Medication> GetAllPharmMedications()
        {
            var PharmMedication = new List<Medication>();
            PharmMedication = _dbContext.Medication.ToList();
            return PharmMedication;
        }


        public List<Medication> GetAllDosageForms()
        {
            var DF = new List<Medication>();
            DF = _dbContext.Medication.ToList();
            return DF;
        }


        public List<Medication> GetAllSchedules()
        {
            var schedule = new List<Medication>();
            schedule = _dbContext.Medication.ToList();
            return schedule;
        }


        //[HttpGet]
        //public JsonResult GetAllMedicationsForPrescription(int prescriptionId)
        //{
        //    // Fetch the prescribed medications for a given PrescriptionID
        //    var medications = (from pr in _dbContext.Prescription
        //                       join mis in _dbContext.MedicationInstructions
        //                       on pr.PrescriptionID equals mis.PrescriptionID
        //                       join meds in _dbContext.Medication
        //                       on mis.MedicationID equals meds.MedicationID
        //                       where pr.PrescriptionID == prescriptionId
        //                       select new
        //                       {
        //                           medicationID = meds.MedicationID,
        //                           medicationName = meds.MedicationName,
        //                           prescribedQuantity = mis.Quantity // Handling null if Quantity is nullable
        //                       }).ToList();

        //    // Return as JSON for front-end consumption
        //    return Json(medications);
        //}
        [HttpGet]
        public JsonResult GetAllMedicationsForPrescription(int prescriptionId)
        {
            var medications = (from pr in _dbContext.Prescription
                               join mis in _dbContext.MedicationInstructions
                               on pr.PrescriptionID equals mis.PrescriptionID
                               join meds in _dbContext.Medication
                               on mis.MedicationID equals meds.MedicationID
                               where pr.PrescriptionID == prescriptionId
                               select new
                               {
                                   medicationID = meds.MedicationID,
                                   medicationName = meds.MedicationName

                               }).ToList();

            return Json(medications);
        }
        [HttpGet]
        public IActionResult GetAdministeredQuantities(int prescriptionId, int admittedPatientID)
        {
            var administeredQuantities = _dbContext.AdministerMedication
                .Where(a => a.PrescriptionID == prescriptionId && a.AdmittedPatientID == admittedPatientID)
                .GroupBy(a => a.MedicationID)
                .Select(g => new
                {
                    MedicationID = g.Key,
                    TotalAdministered = g.Sum(a => a.AdministerQuantity)
                })
                .ToList();

            return Json(administeredQuantities);
        }


        #endregion
    }
}
