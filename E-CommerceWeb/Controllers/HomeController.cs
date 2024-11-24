using Microsoft.AspNetCore.Mvc;
using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models.ViewModels;

namespace Bulky.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.category.GetAll();
            var featuredProducts = (await _unitOfWork.product
                .GetAll(includeProperties: "Category"))
                .Take(4);

            var viewModel = new LandingPageVM
            {
                Categories = categories,
                FeaturedProducts = featuredProducts
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}