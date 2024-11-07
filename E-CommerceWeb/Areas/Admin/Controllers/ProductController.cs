using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var products = await _unitOfWork.product.GetAll();
            var pagedProducts = products.ToPagedList(pageNumber, pageSize);
            return View(pagedProducts);
        }

        public IActionResult create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var products = await _unitOfWork.product.GetAll();
            var productExist = products.Any(p => string.Equals(p.Title, product.Title, StringComparison.OrdinalIgnoreCase));

            if (productExist)
            {
                ModelState.AddModelError("Title", "Product Title already exists!");
            }

            if (ModelState.IsValid)
            {
                await _unitOfWork.product.Add(product);
                _unitOfWork.save();
                TempData["Success"] = "Product Added Successfully!";
                return RedirectToAction("Index");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);



            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var productFromDb = await _unitOfWork.product.Get(c => c.Id == id);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            var existingProduct = await _unitOfWork.product.Get(c => c.Id == product.Id, asNoTracking: true);

            if (existingProduct != null && !string.Equals(existingProduct.Title, product.Title, StringComparison.OrdinalIgnoreCase))
            {
                var products = await _unitOfWork.product.GetAll();
                var productExist = products.Any(c => string.Equals(c.Title, product.Title, StringComparison.OrdinalIgnoreCase));

                if (productExist)
                {
                    ModelState.AddModelError("Title", "Product Title already exists!");
                }
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.product.update(product);
                _unitOfWork.save();
                TempData["Success"] = "Product Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var productFromDb = await _unitOfWork.product.Get(c => c.Id == id, asNoTracking: true);
            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            _unitOfWork.product.Delete(product);
            _unitOfWork.save();
            TempData["Success"] = "Product Deleted Successfully!";
            return RedirectToAction("Index");

        }
    }
}
