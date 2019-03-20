using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using UsedGoodApp.Models;
using UsedGoodApp.Infrastructure;
using System.Threading.Tasks;

namespace UsedGoodApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        //TODO Починить выдачу продуктов (колво и скип)
        //TODO Починить вьюшки для ролей (убрать выпадающие списки)

        private ProductsDb db = new ProductsDb();

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    filterContext.ExceptionHandled = true;
        //    filterContext.Result = RedirectToAction("Index", "Error");
        //}

        // GET: Product
        [AllowAnonymous]
        public ActionResult Index()
        {
            Session["RowCount"] = 100;
            CommonDropDownList();
            return View();
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin,Moderator")]
        public ActionResult Create()
        {
            CommonDropDownList();
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel product)
        {
            CommonDropDownList();
            if (ModelState.IsValid)
            {
                var dbProduct = new Product();
                CreateDbProduct(product, dbProduct);
                db.Products.Add(dbProduct);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Moderator,Rater,Partner")]
        [HttpPost]
        public ActionResult Edit(IEnumerable<JsonEditViewModel> products)
        {
            //Токен тут
            ValidateRequestHeader(Request);
            if (products == null || products.Count() == 0)
                return new EmptyResult();

            var _products = GetProductsAll();

            foreach (var item in products)
            {
                Product product = _products.FirstOrDefault(p => p.Id == Convert.ToInt32(item.Id));
                if (product == null)
                    continue;
                else
                {
                    JsonUpdateDbProduct(item, product);
                    db.Entry(product).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return new EmptyResult();
        }

        // POST: Product/Delete/5
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string ids)
        {
            ValidateRequestHeader(Request);
            if (string.IsNullOrEmpty(ids) || ids == "")
            {
                throw new HttpException(400, String.Empty);
            }
            var splited = ids.TrimEnd(' ').Split(' ');
            foreach (var item in splited)
            {
                int id = Convert.ToInt32(item);
                Product product = db.Products.Include(p => p.Price).Include(p => p.Description).Include(p => p.Repair).FirstOrDefault(p => p.Id == id);
                if (item != null)
                {
                    if (product.Price != null)
                        db.Prices.Remove(db.Prices.FirstOrDefault(p => p.Id == product.Price.Id));
                    if (product.Description != null)
                        db.ProductDescriptions.Remove(db.ProductDescriptions.FirstOrDefault(d => d.Id == product.Description.Id));
                    if (product.Repair == null)
                        db.Repairs.Remove(db.Repairs.FirstOrDefault(r => r.Id == product.Repair.Id));
                    db.Products.Remove(product);
                }
            }
            db.SaveChanges();

            int listLength = 100;
            int _skip = 0;//(int)Session["RowCount"] - listLength;
            var roles = GetCurrentUserRoles();

            var _products = GetList(GetProductsSnippet(_skip, listLength));

            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_TablePartial", _products);
            else if (roles.Contains("Rater"))
                return PartialView("_TableRater", _products);
            else if (roles.Contains("Client"))
                return PartialView("_TableClient", _products.Where(p => string.IsNullOrEmpty(p.Reserved)));
            else if (roles.Contains("Partner"))
                return PartialView("_TablePartner", _products.Where(p => string.IsNullOrEmpty(p.Reserved) || p.Reserved == User.Identity.Name));
            else
                return PartialView("_TableDefault", _products);
        }


        public async Task<ActionResult> NextQuery()
        {
            var rowsCount = Convert.ToInt32(Session["RowCount"]);
            int _nextQueryCount = 100;
            Session["RowCount"] = rowsCount + _nextQueryCount;

            var _products = await Task.Run (() => GetProductsSnippet(rowsCount, _nextQueryCount));
            var _productsViews = await Task.Run(() => GetList(_products));

            var roles = GetCurrentUserRoles();
            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_TablePartial", _productsViews);
            else if (roles.Contains("Rater"))
                return PartialView("_TableRater", _productsViews);
            else if (roles.Contains("Client"))
                return PartialView("_TableClient", _productsViews.Where(p => string.IsNullOrEmpty(p.Reserved)));
            else if (roles.Contains("Partner"))
                return PartialView("_TablePartner", _productsViews.Where(p => string.IsNullOrEmpty(p.Reserved) || p.Reserved == User.Identity.Name));
            else
                return PartialView("_TableDefault", _productsViews);
        }

        private IEnumerable<IndexViewModel> GetList()
        {
            IEnumerable<Product> _products = db.Products.Include(p => p.Description)
            .Include(p => p.Price)
            .Include(p => p.Repair)
            .Include(p => p.Category)
            .Include(p => p.Category.Parent)
            .Include(p => p.Warehouse)
            .Include(p => p.Category.Description).ToList();

            IList<IndexViewModel> _productViews = new List<IndexViewModel>();

            foreach (var item in _products)
            {
                _productViews.Add(IndexViewProduct(item));
            }
            //(IEnumerable<ProductViewModel>)_products = _products.Take(4);
            if (!string.IsNullOrEmpty(Session["filteredValue"] as string))
            {
                string idString = Session["filteredValue"] as string;
                string[] id = idString.Split(' ');
                IList<IndexViewModel> _searchProducts = new List<IndexViewModel>();
                foreach (var value in id)
                {
                    int prodId = 0;
                    if (Int32.TryParse(value, out prodId))
                    {
                        var product = _productViews.FirstOrDefault(p => p.Id == prodId);
                        if (product != null)
                            _searchProducts.Add(product);
                    }
                }

                _productViews = _searchProducts;
            }

            CommonDropDownList();

            return _productViews;
        }

        private IEnumerable<IndexViewModel> GetList(IEnumerable<Product> products)
        {
            IList<IndexViewModel> _productViews = new List<IndexViewModel>();

            foreach (var item in products)
            {
                _productViews.Add(IndexViewProduct(item));
            }
            //(IEnumerable<ProductViewModel>)_products = _products.Take(4);
            if (!string.IsNullOrEmpty(Session["filteredValue"] as string))
            {
                string idString = Session["filteredValue"] as string;
                string[] id = idString.Split(' ');
                IList<IndexViewModel> _searchProducts = new List<IndexViewModel>();
                foreach (var value in id)
                {
                    int prodId = 0;
                    if (Int32.TryParse(value, out prodId))
                    {
                        var product = _productViews.FirstOrDefault(p => p.Id == prodId);
                        if (product != null)
                            _searchProducts.Add(product);
                    }
                }

                _productViews = _searchProducts;
            }

            CommonDropDownList();

            return _productViews;
        }

        [HttpPost]
        public ActionResult Search(JsonSearchView search)
        {
            if (search == null)
                RedirectToAction("RenderTable");
            SearchView _search = ParseJsonToSearch(search);

            var products = from p in db.Products.Include(p => p.Description).Include(p => p.Category).Include(p => p.Category.Parent).Include(p => p.Warehouse).ToList()
                           where Category(p) && Warehouse(p) && Status(p)
                           select p;

            bool Category(Product p)
            {
                if (_search.Category != null)
                {
                    if (p.Category == null)
                        return false;
                    else
                    {
                        if (p.Category.Id == _search.Category || (p.Category.Parent != null && p.Category.Parent.Id == _search.Category))
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return true;
            }

            bool Warehouse(Product p)
            {
                if (_search.Warehouse != null)
                {
                    if (p.Warehouse == null)
                        return false;
                    else
                        return p.Warehouse.Id == _search.Warehouse;
                }
                else
                    return true;
            }

            bool Status(Product p)
            {
                if (_search.Status != null)
                {
                    if (p.Description == null)
                        return false;
                    else
                        return p.Description.StatusId == _search.Status;
                }
                else
                    return true;
            }

            string id = String.Empty;
            if (!SearchEmpty(search))
            {
                foreach (var item in products)
                {
                    id += item.Id + " ";
                }
                if (id == "")
                    Session["filteredValue"] = "NotFound";
                else
                    Session["filteredValue"] = id;
            }
            else
                Session["filteredValue"] = string.Empty;

            var roles = GetCurrentUserRoles();
            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_TablePartial", GetList());
            else if (roles.Contains("Rater"))
                return PartialView("_TableRater", GetList());
            else if (roles.Contains("Client"))
                return PartialView("_TableClient", GetList().Where(p => string.IsNullOrEmpty(p.Reserved)));
            else if (roles.Contains("Partner"))
                return PartialView("_TablePartner", GetList().Where(p => string.IsNullOrEmpty(p.Reserved) || p.Reserved == User.Identity.Name));
            else
                return PartialView("_TableDefault", GetList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Helper Methods
        private IEnumerable<string> GetCurrentUserRoles()
        {
            IList<string> roles = new List<string>();
            var userManager = Request.GetOwinContext().GetUserManager<IdentityUserModel.ApplicationUserManager>();

            IdentityUserModel.ApplicationUser user = userManager.FindByName(User.Identity.Name);
            if (user != null)
                roles = userManager.GetRoles(user.Id.ToString());
            return roles;
        }

        private void CommonDropDownList()
        {
            ViewBag.Categories = Helpers.GetCategories(db.Categories.Include(c => c.Description).ToList());
            ViewBag.SubCategories = Helpers.GetSubCategories(db.Categories.Include(c => c.Description).ToList());
            ViewBag.Warehouses = Helpers.GetWarehouses(db.Warehouses.ToList());
            ViewBag.Statuses = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Value = "",
                    Text = ""
                },
                new SelectListItem
                {
                    Value = Convert.ToString(1),
                    Text = "Не готов"
                },
                new SelectListItem
                {
                    Value = Convert.ToString(2),
                    Text = "Подготовлен"
                },
                new SelectListItem
                {
                    Value = Convert.ToString(3),
                    Text = "Фото"
                },
                new SelectListItem
                {
                    Value = Convert.ToString(4),
                    Text = "На продажу"
                },
                new SelectListItem
                {
                    Value = Convert.ToString(5),
                    Text = "Продан"
                }
            };
            ViewBag.Reserved = new List<SelectListItem>() { new SelectListItem { Value = "0", Text = "" }, new SelectListItem { Value = "1", Text = "Зарезервировано" } };
        }
        //TODO Заменить всет такие вызовы на этот метод

        private IEnumerable<Product> GetProductsAll()
        {
            return db.Products.Include(p => p.Description)
            .Include(p => p.Price)
            .Include(p => p.Repair)
            .Include(p => p.Category)
            .Include(p => p.Category.Parent)
            .Include(p => p.Warehouse)
            .Include(p => p.Category.Description).ToList();
        }

        private IEnumerable<Product> GetProductsSnippet(int start, int end)
        {
            return db.Products.Include(p => p.Description)
            .Include(p => p.Price)
            .Include(p => p.Repair)
            .Include(p => p.Category)
            .Include(p => p.Category.Parent)
            .Include(p => p.Warehouse)
            .Include(p => p.Category.Description).OrderBy(p => p.Id).Skip(start).Take(end).ToList();
        }

        private bool SearchEmpty(JsonSearchView filter)
        {
            bool _empty = false;
            if (string.IsNullOrEmpty(filter.Warehouse) && string.IsNullOrEmpty(filter.Category) && string.IsNullOrEmpty(filter.Status))
                _empty = true;
            return _empty;
        }

        private SearchView ParseJsonToSearch(JsonSearchView json)
        {
            SearchView search = new SearchView();
            if (!string.IsNullOrEmpty(json.Category))
                search.Category = Convert.ToInt32(json.Category);
            if (!string.IsNullOrEmpty(json.Warehouse))
                search.Warehouse = Convert.ToInt32(json.Warehouse);
            if (!string.IsNullOrEmpty(json.Status))
                search.Status = Convert.ToInt32(json.Status);
            return search;
        }

        private void CreateDbProduct(CreateViewModel viewProduct, Product dbProduct)
        {
            DateTime arrivalDate;

            int categoryId = 0;

            if (!string.IsNullOrEmpty(viewProduct.Name))
                dbProduct.Description = new ProductDescription { Name = viewProduct.Name };

            if (DateTime.TryParse(viewProduct.ArrivalDate, out arrivalDate))
                dbProduct.ArrivalDate = DateTime.Parse(viewProduct.ArrivalDate);

            if (!string.IsNullOrEmpty(viewProduct.SubCategoryId))
            {
                categoryId = Convert.ToInt32(viewProduct.SubCategoryId.Split('_')[0]);
                dbProduct.Category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
            }
            else if (viewProduct.CategoryId != null)
                dbProduct.Category = db.Categories.FirstOrDefault(c => c.Id == viewProduct.CategoryId);

            if (viewProduct.WarehouseId != null)
                dbProduct.Warehouse = db.Warehouses.FirstOrDefault(w => w.Id == viewProduct.WarehouseId);

            if (viewProduct.PurchasePrice != null)
                dbProduct.Price = new Price { Purchase = (double)viewProduct.PurchasePrice };

            dbProduct.IsOutOfUse = viewProduct.IsOutOfUse;

            if (!string.IsNullOrEmpty(viewProduct.IssueDescription))
            {
                if (dbProduct.Description == null)
                    dbProduct.Description = new ProductDescription { Issue = viewProduct.IssueDescription };
                else
                    dbProduct.Description.Issue = viewProduct.IssueDescription;
            }
        }

        //Global Update Method
        private void JsonUpdateDbProduct(JsonEditViewModel jsonProduct, Product dbProduct)
        {
            var role = GetCurrentUserRoles();
            if (role.Contains("Admin") || role.Contains("Moderator"))
                JsonUpdateDbProductAdmin(jsonProduct, dbProduct);
            else if (role.Contains("Rater"))
                JsonUpdateDbProductRater(jsonProduct, dbProduct);
            else if (role.Contains("Partner"))
                JsonUpdateDbProductPartner(jsonProduct, dbProduct);
        }

        //Update Rater
        private void JsonUpdateDbProductRater(JsonEditViewModel jsonProduct, Product dbProduct)
        {
            double price;

            if (!string.IsNullOrEmpty(jsonProduct.IssueDescription))
            {
                if (dbProduct.Description == null)
                    dbProduct.Description = new ProductDescription();
                dbProduct.Description.Issue = jsonProduct.IssueDescription;
            }

            if (!string.IsNullOrEmpty(jsonProduct.Price))
            {
                if (dbProduct.Price == null)
                    dbProduct.Price = new Price();
                if (double.TryParse(jsonProduct.Price, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out price))
                    dbProduct.Price.Base = price;
            }
        }

        //Update Partner
        private void JsonUpdateDbProductPartner(JsonEditViewModel jsonProduct, Product dbProduct)
        {
            if (!string.IsNullOrEmpty(jsonProduct.Reserved))
            {
                if (dbProduct.Description == null)
                    dbProduct.Description = new ProductDescription();
                if (jsonProduct.Reserved != "0")
                    dbProduct.Description.Reserved = User.Identity.Name;
                else
                    dbProduct.Description.Reserved = null;
            }
        }

        //Update Admin
        private void JsonUpdateDbProductAdmin(JsonEditViewModel jsonProduct, Product dbProduct)
        {

            DateTime arrivalDate;
            DateTime saleDate;
            DateTime repairStart;
            DateTime repairEnd;

            double price;
            double salePrice;
            double purchasePrice;

            bool isOutOfUse = false;

            //Product
            if (DateTime.TryParse(jsonProduct.ArrivalDate, out arrivalDate))
                dbProduct.ArrivalDate = arrivalDate;
            if (DateTime.TryParse(jsonProduct.SaleDate, out saleDate))
                dbProduct.SaleDate = saleDate;
            if (bool.TryParse(jsonProduct.IsOutOfUse, out isOutOfUse))
                dbProduct.IsOutOfUse = isOutOfUse;

            // Repair
            if (!string.IsNullOrEmpty(jsonProduct.RepairStartDate) || !string.IsNullOrEmpty(jsonProduct.RepairFinishDate) || jsonProduct.RepairStatus != null || jsonProduct.RepairPersonName != null)
            {
                if (dbProduct.Repair == null)
                    dbProduct.Repair = new Repair();
                if (DateTime.TryParse(jsonProduct.RepairStartDate, out repairStart))
                    dbProduct.Repair.StartDate = repairStart;
                if (DateTime.TryParse(jsonProduct.RepairFinishDate, out repairEnd))
                    dbProduct.Repair.FinishDate = repairEnd;
                dbProduct.Repair.Status = jsonProduct.RepairStatus;
                dbProduct.Repair.PersonName = jsonProduct.RepairPersonName;
            }

            // Price
            if (jsonProduct.Price != null || jsonProduct.SalePrice != null || jsonProduct.PurchasePrice != null)
            {
                if (dbProduct.Price == null)
                    dbProduct.Price = new Price();
                if (double.TryParse(jsonProduct.Price, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out price))
                    dbProduct.Price.Base = price;
                if (double.TryParse(jsonProduct.SalePrice, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out salePrice))
                    dbProduct.Price.Sale = salePrice;
                if (double.TryParse(jsonProduct.PurchasePrice, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out purchasePrice))
                    dbProduct.Price.Purchase = purchasePrice;
            }

            // Description
            if (jsonProduct.Name != null || jsonProduct.Status != null || jsonProduct.IssueDescription != null || jsonProduct.Reserved != null)
            {
                if (dbProduct.Description == null)
                    dbProduct.Description = new ProductDescription();
                dbProduct.Description.Name = jsonProduct.Name;
                dbProduct.Description.StatusId = Convert.ToInt32(jsonProduct.Status);
                dbProduct.Description.Issue = jsonProduct.IssueDescription;
                if (jsonProduct.Reserved != "0")
                    dbProduct.Description.Reserved = User.Identity.Name;
                else
                    dbProduct.Description.Reserved = null;
            }

            // Category
            if (!string.IsNullOrEmpty(jsonProduct.SubCategoryId) || !string.IsNullOrEmpty(jsonProduct.CategoryId))
            {
                int Id = 0;
                if (!string.IsNullOrEmpty(jsonProduct.CategoryId))
                    Id = Convert.ToInt32(jsonProduct.CategoryId);
                if (!string.IsNullOrEmpty(jsonProduct.SubCategoryId))
                    Id = Convert.ToInt32(jsonProduct.SubCategoryId.Split('_')[0]);
                dbProduct.Category = db.Categories.FirstOrDefault(c => c.Id == Id);
            }
            else
                dbProduct.Category = null;

            // Warehouse
            if (!string.IsNullOrEmpty(jsonProduct.WarehouseId))
            {
                int warehouseId = Convert.ToInt32(jsonProduct.WarehouseId);
                dbProduct.Warehouse = db.Warehouses.FirstOrDefault(w => w.Id == warehouseId);
            }
            else
                dbProduct.Warehouse = null;
        }

        private IndexViewModel IndexViewProduct(Product dbProduct)
        {
            var indexView = new IndexViewModel();

            indexView.Id = dbProduct.Id;
            indexView.ArrivalDate = dbProduct.ArrivalDate.ToString("yyyy-MM-dd");
            indexView.SaleDate = dbProduct.SaleDate.ToString("yyyy-MM-dd");
            indexView.IsOutOfUse = dbProduct.IsOutOfUse;

            if (dbProduct.Category != null)
            {
                if (dbProduct.Category.Parent != null)
                {
                    indexView.CategoryId = dbProduct.Category.Parent.Id;
                    indexView.SubCategoryId = dbProduct.Category.Id;
                }
                else
                    indexView.CategoryId = dbProduct.Category.Id;
            }
            if(dbProduct.Warehouse != null)
            {
                indexView.WarehouseId = dbProduct.Warehouse.Id;
            }
            if (dbProduct.Description != null)
            {
                indexView.Name = dbProduct.Description.Name;
                indexView.Status = dbProduct.Description.StatusId;
                indexView.IssueDescription = dbProduct.Description.Issue;
            }               
            if(dbProduct.Price != null)
            {
                indexView.Price = dbProduct.Price.Base;
                indexView.SalePrice = dbProduct.Price.Sale;
                indexView.PurchasePrice = dbProduct.Price.Purchase;
            }          
            if (dbProduct.Repair != null)
            {
                indexView.RepairStatus = dbProduct.Repair.Status;
                indexView.RepairPersonName = dbProduct.Repair.PersonName;
                indexView.RepairStartDate = dbProduct.Repair.StartDate.ToString("yyyy-MM-dd");
                indexView.RepairFinishDate = dbProduct.Repair.FinishDate.ToString("yyyy-MM-dd");
            }

            indexView.Reserved = dbProduct.Description.Reserved;

            return indexView;
        }
        
        [ChildActionOnly]
        public ActionResult RenderMenuButtons()
        {
            var roles = GetCurrentUserRoles();
            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_MenuButtons");
            else
                return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult RenderTable()
        {
            var roles = GetCurrentUserRoles();
            int listLength = 100;
            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_TablePartial", GetList().Take(listLength));
            else if(roles.Contains("Rater"))
                return PartialView("_TableRater", GetList().Take(listLength));
            else if(roles.Contains("Client"))
                return PartialView("_TableClient", GetList().Take(listLength).Where(p => string.IsNullOrEmpty(p.Reserved)));
            else if (roles.Contains("Partner"))
                return PartialView("_TablePartner", GetList().Take(listLength).Where(p => string.IsNullOrEmpty(p.Reserved) || p.Reserved == User.Identity.Name));
            else
                return PartialView("_TableDefault", GetList().Take(listLength));
        }
        
        [ChildActionOnly]
        public ActionResult RenderTableHead()
        {
            var roles = GetCurrentUserRoles();
            if (roles.Contains("Moderator") || roles.Contains("Admin"))
                return PartialView("_TableHeadAdmin");
            else if (roles.Contains("Partner"))
                return PartialView("_TableHeadPartner");
            else
                return PartialView("_TableHeadDefault");
        }
       
        //TokenValidation
        void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = "";
            string formToken = "";

            IEnumerable<string> tokenHeaders = request.Headers.GetValues("RequestVerificationToken");
            if (tokenHeaders != null)
            {
                string[] tokens = tokenHeaders.First().Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }
        #endregion
    }
}
