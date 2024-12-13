using Buljy.DataAccess.Repository.IRepository;
using Buljy.Models;
using Buljy.Models.ViewModels;
using Buljy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using Stripe.Checkout;

namespace E_CommerceWeb.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController( IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public  IActionResult Details(int orderId)
        {
            OrderVM OrderVM = new()
            {
                OrderHeader =  _unitOfWork.orderHeader.Get(o => o.Id == orderId, includeProperties: "ApplicationUser").Result,
                OrderDetails =  _unitOfWork.orderDetails.GetAll(D => D.OrderHeaderId == orderId, includeProperties: "product").Result
            };

            return View(OrderVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole + "," + SD.AdminRole)]
        public IActionResult StartProcessing(OrderVM orderVM)
        {
            _unitOfWork.orderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.save();
            TempData["Success"] = "Order is in process";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole + "," + SD.AdminRole)]

        public IActionResult ShipOrder(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.orderHeader.Get(o => o.Id == orderVM.OrderHeader.Id).Result;
            orderHeader.Carrier = orderVM.OrderHeader.Carrier;
            orderHeader.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.orderHeader.update(orderHeader);
            _unitOfWork.save();
            TempData["Success"] = "Order Shipped successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole + "," + SD.AdminRole)]
        public IActionResult CancelOrder(OrderVM orderVM)
        {
            var orderHeader = _unitOfWork.orderHeader.Get(o => o.Id == orderVM.OrderHeader.Id).Result;

            if (orderHeader.PaymentStatus == SD.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.orderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.save();
            TempData["Success"] = "Order cancelled successfully";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult PayNow(OrderVM orderVM)
        {
            orderVM.OrderHeader = _unitOfWork.orderHeader.Get(o => o.Id == orderVM.OrderHeader.Id, includeProperties: "ApplicationUser").Result;
            orderVM.OrderDetails= _unitOfWork.orderDetails.GetAll(d=>d.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: "product").Result;

            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"Admin/order/details?orderId={orderVM.OrderHeader.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderVM.OrderDetails)
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
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new Stripe.Checkout.SessionService();
            var session = service.Create(options);

            _unitOfWork.orderHeader.UpdateStripePaymentId(
                orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.save();

            Response.Headers.Add("Location", session.Url);
            return StatusCode(303);

        }


        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.orderHeader.Get(u => u.Id == orderHeaderId).Result;
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();

                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.orderHeader.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.orderHeader.UpdateStripePaymentId(orderHeaderId, session.PaymentIntentId, session.PaymentIntentId);
                    _unitOfWork.save();
                }

            }
            
            return View(orderHeaderId);
        }








        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> UpdateOrder(OrderVM orderVM)
        {
            if (orderVM == null || orderVM.OrderHeader == null)
            {
                TempData["Error"] = "Invalid data submitted.";
                return RedirectToAction("Details");
            }

            // Fetch the order from the database
            var orderFromDB = await _unitOfWork.orderHeader.Get(o => o.Id == orderVM.OrderHeader.Id);

            if (orderFromDB == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Details");
            }

            // Update properties
            orderFromDB.Name = orderVM.OrderHeader.Name;
            orderFromDB.Phone = orderVM.OrderHeader.Phone;
            orderFromDB.StreetAddress = orderVM.OrderHeader.StreetAddress;
            orderFromDB.City = orderVM.OrderHeader.City;
            orderFromDB.State = orderVM.OrderHeader.State;
            orderFromDB.PostalCode = orderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
            {
                orderFromDB.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
            }

            if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
            {
                orderFromDB.Carrier = orderVM.OrderHeader.Carrier;
            }

            // Save changes
            _unitOfWork.orderHeader.update(orderFromDB);
             _unitOfWork.save();

            TempData["Success"] = "Order updated successfully";
            return RedirectToAction("Details", new { orderId = orderFromDB.Id });
        }


        

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(string status)
        {
            IEnumerable<OrderHeader> orders;


            if(User.IsInRole("Admin")|| User.IsInRole("Employee"))
            {
                orders = await _unitOfWork.orderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity= (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                orders = await _unitOfWork.orderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }


            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                switch (status.ToLower())
                {
                    case "pending":
                        orders = orders.Where(o => o.OrderStatus == SD.StatusPending);
                        break;
                    case "inprocess":
                        orders = orders.Where(o => o.OrderStatus == SD.StatusInProcess);
                        break;
                    case "completed":
                        orders = orders.Where(o => o.OrderStatus == SD.StatusShipped);
                        break;
                    case "approved":
                        orders = orders.Where(o => o.PaymentStatus == SD.PaymentStatusApproved);
                        break;
                }
            }
            var orderList = orders.Select(o => new
            {
                o.Id,
                o.OrderDate,
                o.OrderTotal,
                o.OrderStatus,
                o.PaymentStatus,
                ApplicationUser = new
                {
                    o.ApplicationUser.FirstName,
                    o.ApplicationUser.LastName,
                    o.ApplicationUser.Email,
                    o.ApplicationUser.PhoneNumber
                }
            });

            return Json(new { data = orderList });
        }


       
    }
}
