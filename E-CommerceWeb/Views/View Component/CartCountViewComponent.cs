using Microsoft.AspNetCore.Mvc;
using Buljy.DataAccess.Repository.IRepository;

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

            var cartCount = _unitOfWork.shoppingCart
                .GetAll(sc => sc.ApplicationUserId == userId);
               var count= cartCount.Result.Sum(sc => sc.Quantity);

            return Content(count.ToString());
        }
    }
}