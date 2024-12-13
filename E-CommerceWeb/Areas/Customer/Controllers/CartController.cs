using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using E_CommerceWeb.Services;
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
        private readonly ICartService _cartService;

        public CartController(IUnitOfWork unitOfWork, ICartService cartService)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {  
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                shoppingCarts = await _unitOfWork.shoppingCart.GetAll(
                    c => c.ApplicationUserId == userId,
                    includeProperties: "product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.OrderTotal = 0; 

            foreach (var cart in ShoppingCartVM.shoppingCarts)
            {
                cart.product.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.product.Price * cart.Quantity);
            }

            return View(ShoppingCartVM);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, string operation)
        {
            var cart = await _unitOfWork.shoppingCart.Get(
                c => c.Id == id,
                includeProperties: "product");

            if (cart == null)
            {
                TempData["Error"] = "Cart item not found.";
                return RedirectToAction("Index");
            }

            if (operation == "increase")
            {
                if (cart.Quantity < 99)
                {
                    cart.Quantity += 1;
                    _unitOfWork.shoppingCart.update(cart);
                }
                else
                {
                    TempData["Error"] = "Maximum quantity reached.";
                    return RedirectToAction("Index");
                }
            }
            else if (operation == "decrease")
            {
                if (cart.Quantity > 1)
                {
                    cart.Quantity -= 1;
                    _unitOfWork.shoppingCart.update(cart);
                }
                else
                {
                    await _unitOfWork.shoppingCart.Delete(cart);
                }
            }

             _unitOfWork.save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = await _unitOfWork.shoppingCart.Get(
                c => c.Id == cartId,
                includeProperties: "product");

            if (cart == null)
            {
                return NotFound();
            }

            await _unitOfWork.shoppingCart.Delete(cart);
            _unitOfWork.save();

            // Add TempData message for feedback
            TempData["Success"] = "Item removed from cart successfully";

            return RedirectToAction(nameof(Index));
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