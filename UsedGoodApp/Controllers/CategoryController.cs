using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UsedGoodApp.Models;
using UsedGoodApp.Infrastructure;

namespace UsedGoodApp.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class CategoryController : Controller
    {
        private ProductsDb db = new ProductsDb();

        // GET: Category
        public ActionResult Index()
        {
            IList<CategoryIndexViewModel> viewModels = new List<CategoryIndexViewModel>();
            foreach (var item in db.Categories.Include(c => c.Description).ToList())
            {
                viewModels.Add(IndexViewModel(item));
            }

            return View(viewModels);
        }

        // GET: Category/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Category category = db.Categories.Find(id);
        //    if (category == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(category);
        //}

        // GET: Category/Create
        public ActionResult Create()
        {
            ViewBag.Categories = Helpers.GetCategories(db.Categories.Include(c => c.Description));
            return View();
        }

        // POST: Category/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryCreateViewModel category)
        {
            ViewBag.Categories = Helpers.GetCategories(db.Categories.Include(c => c.Description));
            if (ModelState.IsValid)
            {
                var newCategory = CreateCategory(category);
                db.Categories.Add(newCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Include(c => c.Description).Include(c=>c.Parent).FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return HttpNotFound();
            }
            var editViewModel = EditViewModel(category);
            return View(editViewModel);
        }

        // POST: Category/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit (CategoryEditViewModel categoryView)
        {
            //categoryView.Categories = Helpers.GetCategories(db.Categories.Include(c => c.Description));
            if (ModelState.IsValid)
            {
                var category = db.Categories.Include(c => c.Description).ToList().FirstOrDefault(c => c.Id == categoryView.Id);
                EditCategory(category, categoryView);
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(categoryView);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Include(c => c.Description).FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private CategoryIndexViewModel IndexViewModel(Category category)
        {
            return new CategoryIndexViewModel
            {
                Id = category.Id,
                Name = category.Description.Name,
                Parent = category.Parent != null ? category.Parent.Description.Name : String.Empty,
                //Parent = category.Parent.Description.Name != null ? category.Parent.Id : 0
            };
        }       


        //Тут немного косстыль с полученичем Парента
        private CategoryEditViewModel EditViewModel(Category category)
        {
            return new CategoryEditViewModel
            {
                Id = category.Id,
                Name = category.Description.Name,
                ParentId = category.Parent != null ? category.Parent.Id : 0,
                //Parent = category.Parent != null ? (db.Categories.Include(c => c.Description).FirstOrDefault(c => c.Id == category.Parent.Id)).Description.Name : String.Empty,  //category.Parent.Description.Name : String.Empty,
                Categories = Helpers.GetCategories(db.Categories.Include(c => c.Description))
            };
        }

        private void EditCategory(Category category, CategoryEditViewModel modelView)
        {
            //category.Parent = db.Categories.FirstOrDefault(c => c.Description.Name == modelView.Parent);
            category.Parent = db.Categories.FirstOrDefault(c => c.Id == modelView.ParentId);
            category.Description.Name = modelView.Name; 
        }

        private Category CreateCategory(CategoryCreateViewModel modelView)
        {
            Category category = new Category();
            category.Parent = db.Categories.FirstOrDefault(c => c.Id == modelView.ParentId);
            category.Description = db.CategoryDescriptions.FirstOrDefault(c => c.Name == modelView.Name);
            if (category.Description == null)
                category.Description = new CategoryDescription { Name = modelView.Name };
            return category;
        }
    }
}
