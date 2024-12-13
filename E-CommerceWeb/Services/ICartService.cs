namespace E_CommerceWeb.Services
{
    public interface ICartService
    {
        Task<CartUpdateResult> UpdateCartItemQuantityAsync(int id, string operation);
        Task<CartInfo> GetCartInfoAsync();
    }

    public class CartUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? CartTotal { get; set; }
    }

    public class CartInfo
    {
        public int ItemCount { get; set; }
        public decimal CartTotal { get; set; }
    }
}
