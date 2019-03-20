using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UsedGoodApp.Models;

namespace UsedGoodApp.Infrastructure
{
    public class Helpers
    {
        public static IEnumerable<SelectListItem> GetCategories(IEnumerable<Category> categories)
        {
            var _categories = new List<SelectListItem>();
            _categories.Add(new SelectListItem { Value = String.Empty, Text = String.Empty });
            foreach (var category in categories)
            {
                if (category.Parent == null)
                {
                    _categories.Add(new SelectListItem
                    {
                        Value = Convert.ToString(category.Id),
                        Text = category.Description.Name
                        //Group = GetSubCategories(categories.Where(c => c.Id == category.Id), category.Id)
                    });
                }
            }
            return _categories;
        }

        public static IEnumerable<SelectListItem> GetSubCategories(IEnumerable<Category> categories)
        {
            var subCategories = new List<SelectListItem>();
            subCategories.Add(new SelectListItem { Value = String.Empty, Text = String.Empty });
            foreach (var sub in categories)
            {
                if (sub.Parent != null)
                {
                    subCategories.Add(new SelectListItem
                    {
                        //Value = Convert.ToString(sub.Id) + "_" + Convert.ToString(sub.Parent.Id),
                        Value = $"{sub.Id}_{sub.Parent.Id}",
                        Text = sub.Description.Name
                    });
                }
            }

            return subCategories;
        }

        public static IEnumerable<SelectListItem> GetWarehouses(IEnumerable<Warehouse> warehouses)
        {
            var warehousesList = new List<SelectListItem>();
            warehousesList.Add(new SelectListItem { Value = String.Empty, Text = String.Empty });
            foreach (var warehouse in warehouses)
            {
                warehousesList.Add(new SelectListItem
                {
                   Text = warehouse.Name,
                   Value = Convert.ToString(warehouse.Id)
                });
            }
            return warehousesList;
        }

        public static IEnumerable<SelectListItem> GetGroupCategories(IEnumerable<IGrouping<string, Category>> categories)
        {
            var groupCategories = new List<SelectListItem>();
            groupCategories.Add(new SelectListItem { Value = String.Empty, Text = String.Empty });
            //var categoriesGroup = categories.GroupBy(c => c.Parent.Description.Name);
            foreach (var group in categories)
            {

                SelectListGroup groupName = null; //= new SelectListGroup { Name = group.Key };
                if (!string.IsNullOrEmpty(group.Key))
                {
                    groupName = new SelectListGroup { Name = group.Key };
                    foreach (var item in group)
                    {
                        groupCategories.Add(new SelectListItem
                        {
                            Text = item.Description.Name,
                            Value = $"{item.Id}_{item.Parent.Id}",
                            Group = groupName
                        });
                    }
                }
                else
                {
                    foreach (var item in group)
                    {
                        groupCategories.Add(new SelectListItem
                        {
                            Text = item.Description.Name,
                            Value = $"{item.Id}_0"
                        });
                    }
                }              
            }
            return groupCategories;
        }
    }   
}