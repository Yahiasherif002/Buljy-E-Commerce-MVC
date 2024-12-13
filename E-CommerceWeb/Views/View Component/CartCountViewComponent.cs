using Microsoft.AspNetCore.Mvc;
using Buljy.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Buljy.Utility;

namespace Buljy.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartCountViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Content("0");
            }
            if (userId != null)
            {
                var cartItems = await _unitOfWork.shoppingCart.GetAll(sc => sc.ApplicationUserId == userId);
                var count = cartItems.Count();

                HttpContext.Session.SetInt32(SD.SessionCart, count);
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }




        }
    }
}


