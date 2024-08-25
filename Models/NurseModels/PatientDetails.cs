﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.NurseModels
{
    public class PatientDetails
    {
        [Key]
        public int PatientDetailsID {  get; set; }
        [Required]
        public int AdmittedPatientID {  get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateOnly DateOfBirth{ get; set;}
        [Required]
        public int AddressID {  get; set; }
        [Required]
        public int CityID { get; set; }
        [Required]
        public int ProvinceID { get; set; }
        [Required]
        public int SuburbID { get; set; }
        [Required]
        public string Street { get; set; }

    }
}
