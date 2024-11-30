using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Buljy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PagedList;
using System.Linq;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IActionResult> Index(string? searchParam, string? sortOrder, int? page, int? pageSize)
        {
            int defaultPageSize = 10;
            int size = (pageSize > 0 ? pageSize : defaultPageSize) ?? defaultPageSize;
            int pageNumber = (page > 0 ? page : 1) ?? 1;

            // Set up ViewData for maintaining state in the view
            ViewData["CurrentSearch"] = searchParam;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["OrderSortParam"] = sortOrder == "order" ? "order_desc" : "order";

            // Retrieve data from the repository
            var companies = await _unitOfWork.company.GetAll();

            // Filter based on the search parameter
            if (!string.IsNullOrEmpty(searchParam))
            {
                companies = companies.Where(c => c.Name.Contains(searchParam, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "name_desc":
                    companies = companies.OrderByDescending(c => c.Name);
                    break;
               
                default:
                    companies = companies.OrderBy(c => c.Name);
                    break;
            }

            // Apply pagination
            var pagedCompanies = companies.ToPagedList(pageNumber, size);

            return View(pagedCompanies);
        }



        public async Task<IActionResult> Upsert(int? id)
        {
            Company company = new Company();
            if (id == 0 || id == null)
            {
                return View(company);
            }
            else
            {
                company = await _unitOfWork.company.Get(u => u.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company)
        {
            if (company == null )
            {
                return BadRequest("Company data is invalid.");
            }


            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    await _unitOfWork.company.Add(company);
                    TempData["Success"] = "Company Added Successfully!";
                }
                else
                {
                    _unitOfWork.company.update(company);
                    TempData["Success"] = "Company Updated Successfully!";
                }
                _unitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public async Task<IActionResult> Details(int id)
        {
            var company = await _unitOfWork.company.Get(u => u.Id == id);
            if (company == null)
            {
                TempData["Error"] = "Error retrieving company!";
                return NotFound();
            }
            return View(company);
        }

        public async Task<IActionResult> Delete(int id)
        {

            if (id == null || id == 0)
                return NotFound();

            var CompanyFromDb = await _unitOfWork.company.Get(c => c.Id == id, asNoTracking: true);
            if (CompanyFromDb == null)
            {
                return NotFound();
            }

            return View(CompanyFromDb);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(Company company)
        {
            if (company == null)
            {
                TempData["Error"] = "Error deleting company!";
                return NotFound();
            }
            _unitOfWork.company.Delete(company);
            _unitOfWork.save();
            TempData["Success"] = "Company deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
