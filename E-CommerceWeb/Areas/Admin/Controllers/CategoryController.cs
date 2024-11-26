using Buljy.DataAccess.Data;
using Buljy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Buljy.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Buljy.Utility;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.category.GetAllWithP();
            
            return View("Index", categories);
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            var categories = await _unitOfWork.category.GetAll();
            var categoryExist = categories.Any(c => string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase));

            if (categoryExist)
            {
                ModelState.AddModelError("Name", "Category Name already exists!");
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.category.Add(category);
                _unitOfWork.save();
                TempData["Success"] = "Category Added Successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = await _unitOfWork.category.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            var existingCategory = await _unitOfWork.category.Get(c => c.Id == category.Id,asNoTracking:true);

            if (existingCategory != null && !string.Equals(existingCategory.Name, category.Name, StringComparison.OrdinalIgnoreCase))
            {
                var categories = await _unitOfWork.category.GetAll();
                var categoryExist = categories.Any(c => string.Equals(c.Name, category.Name, StringComparison.OrdinalIgnoreCase));

                if (categoryExist)
                {
                    ModelState.AddModelError("Name", "Category Name already exists!");
                }
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.category.update(category);
                _unitOfWork.save();
                TempData["Success"] = "Category Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var categoryFromDb = await _unitOfWork.category.Get(c => c.Id == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var category = await _unitOfWork.category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.category.Delete(category);
            _unitOfWork.save();
            TempData["Success"] = "Category Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}
