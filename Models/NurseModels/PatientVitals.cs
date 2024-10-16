﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientVitals
    {
        [Key]
        public int PatientVitalsID { get; set; }

        [Required]
        public int PatientID { get; set; }
   
        [Required]
        public int Height { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public int SystolicBloodPressure { get; set; }
        [Required]

        public int DiastolicBloodPressure { get; set; }
        [Required]
        public int HeartRate { get; set; }

        [Required]
        public int BloodOxygen { get; set; }
        [Required]
        public int Respiration { get; set; }
        [Required]
        public int BloodGlucoseLevel { get; set; }
        [Required]
        public int Temperature { get; set; }
        [Required]
        public TimeOnly time { get; set; }
        //public string GetFormattedTime()
        //{
        //    return time.ToString("HH:mm:ss");
        //}
    }
}
