using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            int defaultPageSize = 10;
            int size = pageSize ?? defaultPageSize;  // Default to 10 if pageSize is null
            int pageNumber = page ?? 1;  // Default to page 1 if not specified

            var products = await _unitOfWork.product.GetAll(includeProperties: "Category");
            var pagedProducts = products.ToPagedList(pageNumber, size);  // Pass the pageSize here
            ViewBag.PageSize = size;  // Store the selected page size for the view

            return View(pagedProducts);
        }



        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                ListItems = (await _unitOfWork.category.GetAll()).Select(U => new SelectListItem
                {
                    Text = U.Name,
                    Value = U.Id.ToString()
                }),
                Product = new Product()
            };
            // Create
            if (id == 0 || id == null)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = await _unitOfWork.product.Get(u => u.Id == id);
                return View(productVM);

            }
        }

        [HttpPost]

        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file) 
        {
            
            if (productVM == null || productVM.Product == null)
            {
                return BadRequest("Product data is invalid.");
            }

            // Remove file validation if it's null (e.g., when editing without changing the image)
            if (file == null && productVM.Product.Id != 0)
            {
                ModelState.Remove("file"); 
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Handle image upload only if a new file is uploaded
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Save the new file and update the image URL
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                else
                {
                    // Preserve existing ImageUrl if no new file is uploaded (for edit only)
                    if (productVM.Product.Id != 0)
                    {
                        var existingProduct = await _unitOfWork.product.Get(p => p.Id == productVM.Product.Id);
                        productVM.Product.ImageUrl = existingProduct?.ImageUrl;
                    }
                }

                // Add or update the product
                if (productVM.Product.Id != 0)
                {
                    _unitOfWork.product.update(productVM.Product);
                    TempData["Success"] = "Product Updated Successfully!";
                }
                else
                {
                    _unitOfWork.product.Add(productVM.Product);
                    TempData["Success"] = "Product Added Successfully!";
                }

                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {
                // If there are validation errors, populate ListItems for the view
                productVM.ListItems = (await _unitOfWork.category.GetAll()).Select(U => new SelectListItem
                {
                    Text = U.Name,
                    Value = U.Id.ToString()
                });
            }

            // Return the view with the current productVM data
            return View(productVM);
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
        #region API 

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.product.GetAll(includeProperties: "Category");
            return Json(new { data = products }); // Return the list under the 'data' key
        }



        #endregion
    }
}
