using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;

namespace E_CommerceWeb.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartUpdateResult> UpdateCartItemQuantityAsync(int id, string operation)
        {
            var cart = await GetCartItemAsync(id);
            if (cart == null)
            {
                return new CartUpdateResult
                {
                    Success = false,
                    Message = "Cart item not found."
                };
            }

            var result = await UpdateQuantityBasedOnOperationAsync(cart, operation);
            if (result.Success)
            {
                 _unitOfWork.save();
                result.CartTotal = await CalculateCartTotalAsync();
            }

            return result;
        }

        public async Task<CartInfo> GetCartInfoAsync()
        {
            var cartItems = await _unitOfWork.shoppingCart.GetAll(includeProperties: "product");
            return new CartInfo
            {
                ItemCount = cartItems.Count(),
                CartTotal = (decimal)cartItems.Sum(c => c.product.Price * c.Quantity)
            };
        }

        private async Task<ShoppingCart> GetCartItemAsync(int id)
        {
            return await _unitOfWork.shoppingCart.Get(
                c => c.Id == id,
                includeProperties: "product");
        }

        private async Task<CartUpdateResult> UpdateQuantityBasedOnOperationAsync(ShoppingCart cart, string operation)
        {
            switch (operation.ToLower())
            {
                case "increase":
                    return await IncreaseQuantity(cart);
                case "decrease":
                    return await DecreaseQuantity(cart);
                default:
                    return new CartUpdateResult
                    {
                        Success = false,
                        Message = "Invalid operation."
                    };
            }
        }

        private async Task<CartUpdateResult> IncreaseQuantity(ShoppingCart cart)
        {
            cart.Quantity += 1;
            _unitOfWork.shoppingCart.update(cart);

            return new CartUpdateResult
            {
                Success = true,
                Message = "Quantity increased successfully",
                Quantity = cart.Quantity,
                TotalPrice = (decimal?)(cart.product.Price * cart.Quantity)
            };
        }

        private async Task<CartUpdateResult> DecreaseQuantity(ShoppingCart cart)
        {
            if (cart.Quantity > 1)
            {
                cart.Quantity -= 1;
                _unitOfWork.shoppingCart.update(cart);

                return new CartUpdateResult
                {
                    Success = true,
                    Message = "Quantity decreased successfully",
                    Quantity = cart.Quantity,
                    TotalPrice = (decimal?)(cart.product.Price * cart.Quantity)
                };
            }
            else
            {
                await _unitOfWork.shoppingCart.Delete(cart);
                return new CartUpdateResult
                {
                    Success = true,
                    Message = "Item removed from cart",
                    Quantity = 0,
                    TotalPrice = 0
                };
            }
        }

        private async Task<decimal> CalculateCartTotalAsync()
        {
            var cartItems = await _unitOfWork.shoppingCart.GetAll(includeProperties: "product");
            return (decimal)cartItems.Sum(c => c.product.Price * c.Quantity);
        }
    }
}
