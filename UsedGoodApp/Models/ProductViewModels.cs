using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UsedGoodApp.Models
{
    //17 fields (id hidden)

    public class IndexViewModel
    {
        public bool IsChecked { get; set; }

        public int Id { get; set; }

        [Display(Name="Название")]
        public string Name { get; set; }

        [Display(Name = "Дата Получения")]
        [DataType(DataType.Date)]
        public string ArrivalDate { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }       

        [Display(Name = "Подкатегория")]
        public int SubCategoryId { get; set; }

        [Display(Name = "Цена")]
        public double Price { get; set; }

        [Display(Name = "Рабочий")]
        public bool IsOutOfUse { get; set; }

        [Display(Name = "Склад")]
        public int WarehouseId { get; set; }

        [Display(Name = "Статус")]
        public int Status { get; set; }

        [Display(Name = "ДатаПродажи")]
        [DataType(DataType.Date)]
        public string SaleDate { get; set; }

        [Display(Name = "ЦенаПродажи")]
        public double SalePrice { get; set; }

        [Display(Name = "Рем.Статус")]
        public string RepairStatus { get; set; }

        [Display(Name = "Рем.Мастер")]
        public string RepairPersonName { get; set; }

        [Display(Name = "Начало Ремонта")]
        [DataType(DataType.Date)]
        public string RepairStartDate { get; set; }

        [Display(Name = "Окончание Ремонта")]
        [DataType(DataType.Date)]
        public string RepairFinishDate { get; set; }

        [Display(Name = "ЦенаПокупки")]
        public double PurchasePrice { get; set; }

        [Display(Name = "Неисправность")]
        [DataType(DataType.MultilineText)]
        public string IssueDescription { get; set; }

        [Display(Name = "Зарезирвировано")]
        public string Reserved { get; set; }
    }

    public class CreateViewModel
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Категория")]
        public int? CategoryId { get; set; }

        [Display(Name = "Подкатегория")]
        public string SubCategoryId { get; set; }

        [Display(Name = "Склад")]
        public int? WarehouseId { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Цена покупки")]
        public double? PurchasePrice { get; set; }

        [Display(Name = "Дата прибытия")]
        [DataType(DataType.Date)]
        public string ArrivalDate { get; set; }

        [Display(Name = "Рабочий")]
        public bool IsOutOfUse { get; set; }

        [Display(Name = "Неисправность")]
        [DataType(DataType.MultilineText)]
        public string IssueDescription { get; set; }
    }

    public class JsonEditViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ArrivalDate { get; set; }

        public string CategoryId { get; set; }

        public string SubCategoryId { get; set; }

        public string Price { get; set; }

        public string IsOutOfUse { get; set; }

        public string WarehouseId { get; set; }

        public string Status { get; set; }

        public string SaleDate { get; set; }

        public string SalePrice { get; set; }

        public string RepairStatus { get; set; }

        public string RepairPersonName { get; set; }

        public string RepairStartDate { get; set; }

        public string RepairFinishDate { get; set; }

        public string PurchasePrice { get; set; }

        public string IssueDescription { get; set; }

        public string Reserved { get; set; }
    }

    public class JsonSearchView
    {
        public string Status { get; set; }
        public string Category { get; set; }
        public string Warehouse { get; set; }
    }
    public class SearchView
    {
        public int? Status { get; set; }
        public int? Category { get; set; }
        public int? Warehouse { get; set; }
    }
}