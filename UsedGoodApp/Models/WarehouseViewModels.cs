using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UsedGoodApp.Models
{
    public class WarehouseViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Название")]
        public string Name { get; set; }
    }

    public class WarehouseEditViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }

    public class WarehouseCreateViewModel
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}