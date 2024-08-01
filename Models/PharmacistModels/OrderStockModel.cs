﻿using System.ComponentModel.DataAnnotations;

namespace DEMO.Models.PharmacistModels
{
    public class OrderStockModel

    {

        [Key]
        public int OrederID { get; set; }


        [Required]
        public int StockID { get; set; }

        [Required]
        public int Amount { get; set;}

        [Required]
        public string Status { get; set; }

    }
}
