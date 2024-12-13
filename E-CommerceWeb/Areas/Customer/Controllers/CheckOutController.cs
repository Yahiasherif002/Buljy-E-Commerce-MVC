using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models.ViewModels;
using Buljy.Models;
using Buljy.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Stripe.Checkout;

namespace E_CommerceWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CheckoutController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var shoppingCartVM = new ShoppingCartVM
            {
                shoppingCarts = await _unitOfWork.shoppingCart.GetAll(
                    c => c.ApplicationUserId == userId,
                    includeProperties: "product"),
                OrderHeader = new OrderHeader()
            };

            var user = await _unitOfWork.applicationUser.Get(u => u.Id == userId);
            if (user != null)
            {
                shoppingCartVM.OrderHeader.ApplicationUser = user;
                shoppingCartVM.OrderHeader.Name = user.FirstName;
                shoppingCartVM.OrderHeader.Phone = user.PhoneNumber;
                shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
                shoppingCartVM.OrderHeader.City = user.City;
                shoppingCartVM.OrderHeader.State = user.State;
                shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
            }

            foreach (var cart in shoppingCartVM.shoppingCarts)
            {
                cart.Price = (double)GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Quantity;
            }

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(ShoppingCartVM shoppingCartVM)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", shoppingCartVM);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await _unitOfWork.shoppingCart.GetAll(
                c => c.ApplicationUserId == userId,
                includeProperties: "product");

            shoppingCartVM.shoppingCarts = cartItems;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            // Recalculate order total for security
            shoppingCartVM.OrderHeader.OrderTotal = 0;
            foreach (var cart in shoppingCartVM.shoppingCarts)
            {
                cart.Price = (double)GetPriceBasedOnQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Quantity;
            }

            // Set order status and payment status
            var user = await _unitOfWork.applicationUser.Get(u => u.Id == userId);
            bool isAssociatedWithCompany = user?.CompanyId.GetValueOrDefault() != 0;

            shoppingCartVM.OrderHeader.OrderStatus = isAssociatedWithCompany
                ? SD.StatusApproved
                : SD.StatusPending;
            shoppingCartVM.OrderHeader.PaymentStatus = isAssociatedWithCompany
                ? SD.PaymentStatusDelayedPayment
                : SD.PaymentStatusPending;

           
                await _unitOfWork.orderHeader.Add(shoppingCartVM.OrderHeader);
                _unitOfWork.save();

                foreach (var cart in shoppingCartVM.shoppingCarts)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = cart.ProductId,
                        OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                        Price = cart.Price,
                        Count = cart.Quantity
                    };
                    await _unitOfWork.orderDetails.Add(orderDetail);
                }

                _unitOfWork.save();


            if (!isAssociatedWithCompany) // User is not associated with a company
            {
                var domain = Request.Scheme + "://" + Request.Host.Value +"/";
                    var options = new Stripe.Checkout.SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/checkout/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                        CancelUrl = domain + "customer/checkout/index",
                        LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in shoppingCartVM.shoppingCarts)
                    {
                        var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                        {
                            PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100),
                                Currency = "usd",
                                ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.product.Title
                                }
                            },
                            Quantity = item.Quantity
                        };
                        options.LineItems.Add(sessionLineItem);
                    }

                    var service = new Stripe.Checkout.SessionService();
                    var session = service.Create(options);

                    _unitOfWork.orderHeader.UpdateStripePaymentId(
                        shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.save();

                    Response.Headers.Add("Location", session.Url);
                    return StatusCode(303);
                }
                return RedirectToAction("OrderConfirmation", new { id = shoppingCartVM.OrderHeader.Id });
            
            
        }


        private decimal GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Quantity <= 50)
                return (decimal)cart.product.Price;
            if (cart.Quantity <= 100)
                return (decimal)cart.product.Price50;
            return (decimal)cart.product.Price100;
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader=  _unitOfWork.orderHeader.Get(u => u.Id == id).Result;
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();

                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStatus(id, SD.PaymentStatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.orderHeader.UpdateStripePaymentId(id, session.PaymentIntentId, session.PaymentIntentId);
                    _unitOfWork.save();
                }
                HttpContext.Session.Clear();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _unitOfWork.shoppingCart.EmptyUserCartAsync(userId);

            return View(id);
        }
    }
}
