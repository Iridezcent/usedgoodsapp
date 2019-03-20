using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UsedGoodApp.Models;

namespace UsedGoodApp.Controllers
{
    [Authorize(Roles ="Admin,Moderator")]
    public class WarehouseController : Controller
    {
        private ProductsDb db = new ProductsDb();

        // GET: Warehouse
        public ActionResult Index()
        {
            IList<WarehouseViewModel> viewList = new List<WarehouseViewModel>();
            foreach (var warehouse in db.Warehouses.ToList())
            {
                viewList.Add(ViewModel(warehouse));
            }
            return View(viewList);
        }

        // GET: Warehouse/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Warehouse warehouse = db.Warehouses.Find(id);
        //    if (warehouse == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(warehouse);
        //}

        // GET: Warehouse/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Warehouse/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] WarehouseCreateViewModel warehouse)
        {
            if (ModelState.IsValid)
            {
                var newWarehouse = CreateWarehouse(warehouse);
                db.Warehouses.Add(newWarehouse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(warehouse);
        }

        // GET: Warehouse/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            var viewWarehouse = EditViewModel(warehouse);
            return View(viewWarehouse);
        }

        // POST: Warehouse/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] WarehouseEditViewModel warehouse)
        {
            if (ModelState.IsValid)
            {
                var editWarehouse = db.Warehouses.Find(warehouse.Id);
                EditWarehouse(editWarehouse, warehouse);
                db.Entry(editWarehouse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(warehouse);
        }

        // GET: Warehouse/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouse/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Warehouse warehouse = db.Warehouses.Find(id);
            db.Warehouses.Remove(warehouse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region CRUD ViewModels

        private WarehouseViewModel ViewModel (Warehouse warehouse)
        {
            return new WarehouseViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name
            };
        }

        //private WarehouseCreateViewModel CreateViewModel(Warehouse warehouse)
        //{
        //    return new WarehouseCreateViewModel
        //    {
        //        Id = warehouse.Id,
        //        Name = warehouse.Name
        //    };
        //}
        
        private Warehouse CreateWarehouse(WarehouseCreateViewModel model)
        {
            return new Warehouse
            {
                Name = model.Name,
            };
        }

        private WarehouseEditViewModel EditViewModel(Warehouse warehouse)
        {
            return new WarehouseEditViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name
            };
        }

        private void EditWarehouse(Warehouse warehouse, WarehouseEditViewModel warehouseView)
        {
            warehouse.Name = warehouseView.Name;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
