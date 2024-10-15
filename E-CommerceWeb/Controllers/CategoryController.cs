using Buljy.DataAccess.Data;
using Buljy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _db;

        public CategoryController(AppDbContext dbContext)
        {
            _db = dbContext;
        }
        public IActionResult Index()
        {
            var categories = _db.categories.ToList();
            return View("index",categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            var categoryExist = _db.categories.Any(c => c.Name.ToLower() == category.Name.ToLower());

            if (categoryExist)
            {
                ModelState.AddModelError("Name", "Category Name already exists!");

            }

            if (ModelState.IsValid) { 
            _db.categories.Add(category);
            _db.SaveChanges();
                TempData["Success"] = "Category Added Successfully!";
                return RedirectToAction("Index");
            }
            return View();

        }


        public IActionResult Edit(int?id)
        {
            if(id==null || id == 0)
             return NotFound();

            var categoryFromDb = _db.categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }


            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var existingCategory = _db.categories.AsNoTracking().FirstOrDefault(c => c.Id == category.Id);

            if (existingCategory != null && existingCategory.Name.ToLower()!=category.Name.ToLower())
            {

               var categoryExist = _db.categories.Any(c => c.Name.ToLower() == category.Name.ToLower());

                  if (categoryExist)
                  {
                     ModelState.AddModelError("Name", "Category Name already exists!");

                  }
            }

          if (ModelState.IsValid)
          {
                _db.categories.Update(category);
                _db.SaveChanges();
                TempData["Success"] = "Category Updated Successfully!";
                return RedirectToAction("Index");
          }
            return View();

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = _db.categories.Find(id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }


            return View(categoryFromDb);
        }



        [HttpPost , ActionName("Delete")]
        public IActionResult DeletePost(int id)
        {
            Category? Category = _db.categories.Find(id);
            if (Category == null)
            {
                return NotFound();
            }

            _db.categories.Remove(Category);
            _db.SaveChanges();
            TempData["Success"] = "Category Deleted Successfully!";
            return RedirectToAction("Index");


            

        }
    }
}
