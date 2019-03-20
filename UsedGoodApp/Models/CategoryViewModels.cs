using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UsedGoodApp.Models
{
    public class CategoryIndexViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Родитель")]
        public string Parent { get; set; }

        [Display(Name="Название")]
        public string Name { get; set; }
    }

    public class CategoryEditViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Родитель")]
        public int? ParentId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }

    public class CategoryCreateViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Родитель")]
        public int? ParentId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}