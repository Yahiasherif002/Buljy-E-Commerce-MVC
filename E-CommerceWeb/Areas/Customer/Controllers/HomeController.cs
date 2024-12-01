using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace E_CommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
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

        public async Task<IActionResult> Details(int id)
        {

            ShoppingCart cart = new ShoppingCart()
            {
                product = await _unitOfWork.product.Get(includeProperties: "Category", filter: c => c.Id == id),
                ProductId = id
            };
            
            if (cart == null)
            {
                return NotFound(); 
            }

            return View(cart); 
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = await _unitOfWork.shoppingCart.Get(
                c => c.ApplicationUserId == shoppingCart.ApplicationUserId && c.ProductId == shoppingCart.ProductId);

            if (cartFromDb == null)
            {
                var quantity = shoppingCart.Quantity;
                shoppingCart.Id = 0;
                await _unitOfWork.shoppingCart.Add(shoppingCart);
            }
            else
            {
                cartFromDb.Quantity += shoppingCart.Quantity;
                 _unitOfWork.shoppingCart.update(cartFromDb);
            }

            TempData["success"]= "Item has been added to cart";

            _unitOfWork.save();

            return RedirectToAction(nameof(Index));
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
