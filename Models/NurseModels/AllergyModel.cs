﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEMO.Models.NurseModels
{
    public class AllergyModel

    {

        [Key]
        public int AllergyID { get; set; }
        
        [Key]
        public int ActiveingredientID { get; set; }
            
    }
}
