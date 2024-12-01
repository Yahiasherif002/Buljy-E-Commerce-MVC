using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_CommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                shoppingCarts = _unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == UserId, includeProperties: "product").Result,

            };

            foreach (var Cart in ShoppingCartVM.shoppingCarts)
            {
                Cart.Price = GetPriceBasedOnQuantity(Cart);
                ShoppingCartVM.OrderTotal += (Cart.Price * Cart.Quantity);
            }

            return View(ShoppingCartVM);
        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Quantity <= 50)
            {
                return shoppingCart.product.Price;
            }
            else if (shoppingCart.Quantity <= 100)
            {
                return shoppingCart.product.Price50;
            }
            else
            {
                return shoppingCart.product.Price100;
            }
        }
    }
}
