using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Collections.Generic;
using System.Diagnostics;

namespace E_CommerceWeb.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;

        }


    public async Task<IActionResult> Index(int page = 1)
    {
        int pageSize = 8; // Number of products per page
        IEnumerable<Product> products = await _unitOfWork.product.GetAll(includeProperties: "Category");

        // Convert the products to a PagedList
        var pagedProducts = products.ToPagedList(page, pageSize);

        return View(pagedProducts);
    }


    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
