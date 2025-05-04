using Application;
using Domain.Enums;
using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeClient _stripeClient;
        private readonly NotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public PaymentController(IUnitOfWork unitOfWork, StripeClient stripeClient, NotificationService notificationService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _stripeClient = stripeClient;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
        public IActionResult Index()
        {
            var payments = _unitOfWork.Payment.GetAll().ToList();
            foreach (var payment in payments)
            {
                payment.Booking = _unitOfWork.Booking.Get(u => u.Id == payment.BookingId);
                payment.Booking.Trip = _unitOfWork.Trip.Get(u => u.Id == payment.Booking.TripId);
                payment.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == payment.ApplicationUserId);
            }

            return View(payments);
        }


        public ActionResult Details(int paymentId)
        {
            var payment = _unitOfWork.Payment.Get(u => u.Id == paymentId);
            payment.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == payment.ApplicationUserId);
            payment.Booking = _unitOfWork.Booking.Get(u => u.Id == payment.BookingId);
            payment.Booking.Trip = _unitOfWork.Trip.Get(u => u.Id == payment.Booking.TripId);

            return View(payment);
        }

        public IActionResult CreateCheckoutSession(int bookingId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId);
            booking.Trip = _unitOfWork.Trip.Get(t => t.Id == booking.TripId);

            var stripeSettings = _unitOfWork.StripeSettings.GetAll().FirstOrDefault();
            if (stripeSettings == null)
            {
                return BadRequest("Stripe settings not configured.");
            }

            Payment payment = new Payment
            {
                BookingId = booking.Id,
                ApplicationUserId = applicationUser.Id,
                Amount = booking.TotalAmount,
                Status = PaymentStatus.Pending,
                PaymentDate = DateTime.UtcNow,
            };
            _unitOfWork.Payment.Add(payment);
            _unitOfWork.Save();

            var appUrl = _configuration["profiles:http:applicationUrl"];

            var domain = "https://easytrip-dwaed9gffaguaege.canadacentral-01.azurewebsites.net/";
            //var domain = "http://localhost:5263/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/payment/paymentConfirmation?paymentId={payment.Id}",
                CancelUrl = domain + $"User/Booking/Summary?bookingId={payment.BookingId}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            var sessionLineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(payment.Amount * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Easy Trip",
                    },
                },
                Quantity = 1,
            };
            options.LineItems.Add(sessionLineItem);

            var service = new SessionService(_stripeClient);
            Session session = service.Create(options);

            _unitOfWork.Payment.UpdateStripePaymentID(payment.Id, session.Id,
                session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> PaymentConfirmation(int paymentId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            Payment payment =
                _unitOfWork.Payment.Get(u => u.Id == paymentId, includeProperties: "ApplicationUser");
            payment.Booking = _unitOfWork.Booking.Get(u => u.Id == payment.BookingId);

            var trip = _unitOfWork.Trip.Get(u => u.Id == payment.Booking.TripId);
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            var service = new SessionService();
            Session session = service.Get(payment.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                trip.SeatsAvailable -= payment.Booking.NumberOfParticipants;
                if (trip.SeatsAvailable == 0)
                {
                    trip.Status = TripStatus.Sold;
                }
                _unitOfWork.Trip.Update(trip);
                _unitOfWork.Payment.UpdateStripePaymentID(paymentId, session.Id, session.PaymentIntentId);
                _unitOfWork.Payment.UpdateStatus(paymentId, PaymentStatus.Paid);
                _unitOfWork.Save();

                await _notificationService.CreateAndSendNotificationAsync(trip.ApplicationUserId, $" Trip: {trip.Title} - has been booked by {user.FirstName} {user.LastName}",
                    "New trip payment", NotificationType.Payment);
            }

            return RedirectToAction("Confirmation", "Booking",
                new { area = "User", bookingId = payment.BookingId });
        }

        [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
        [HttpPost]
        public IActionResult Delete(int paymentId)
        {
            var payment = _unitOfWork.Payment.Get(u => u.Id == paymentId);

            if (payment == null)
            {
                TempData["error"] = "Payment not found";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Payment.Remove(payment);
            _unitOfWork.Save();

            TempData["success"] = "Payment deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
