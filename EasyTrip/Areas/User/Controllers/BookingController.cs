using Application;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class BookingController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly NotificationService _notificationService;

        public BookingController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IEmailSender emailSender, NotificationService notificationService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _notificationService = notificationService;

        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var bookings = _unitOfWork.Booking.GetAll(b => b.ApplicationUserId == userId).ToList();

            foreach (var booking in bookings)
            {
                booking.Trip = _unitOfWork.Trip.Get(t => t.Id == booking.TripId);
            }

            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int tripId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            var existingBooking = _unitOfWork.Booking.Get(b => b.TripId == tripId && b.ApplicationUserId == userId);
            var trip = _unitOfWork.Trip.Get(t => t.Id == tripId);

            var booking = new Booking();

            if (existingBooking != null && existingBooking.Status == BookingStatus.Pending)
            {
                booking = existingBooking;
            }
            else
            {
                booking = CreateNewBooking(tripId, userId);
                await _notificationService.CreateAndSendNotificationAsync(trip.ApplicationUserId, $" New pending booking for the trip: {trip.Title} - by {user.FirstName}",
                    "New pending booking", NotificationType.Booking);
            }

            return RedirectToAction("Initiate", new { bookingId = booking.Id });
        }


        public IActionResult Initiate(int bookingId)
        {
            var booking = CalculateBooking(bookingId);

            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType))
                .Cast<RoomType>()
                .Select(rt => new SelectListItem
                {
                    Value = ((int)rt).ToString(),
                    Text = rt.ToString()
                })
                .ToList();

            var bookingVM = new BookingVM()
            {
                Booking = booking,
                Participant = new Participant()
            };

            return View("Create", bookingVM);
        }

        public IActionResult Summary(int bookingId)
        {
            var booking = CalculateBooking(bookingId);

            ViewBag.RoomTypes = Enum.GetValues(typeof(RoomType))
                .Cast<RoomType>()
                .Select(rt => new SelectListItem
                {
                    Value = ((int)rt).ToString(),
                    Text = rt.ToString()
                })
                .ToList();

            var bookingVM = new BookingVM()
            {
                Booking = booking,
                Participant = new Participant()
            };

            return View(bookingVM);
        }

        [HttpPost]
        public IActionResult Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Booking.Add(booking);
                _unitOfWork.Save();

                return RedirectToAction("Index", "Participant", booking);
            }

            return View(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId);
            _unitOfWork.Booking.Remove(booking);
            _unitOfWork.Save();

            TempData["success"] = "Booking deleted successfully";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Confirmation(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(p => p.Id == bookingId);
            booking.Status = BookingStatus.Confirmed;
            _unitOfWork.Booking.Update(booking);
            _unitOfWork.Save();

            SendConfirmationEmail(booking.ApplicationUserId, booking.Id);
            booking.Trip = _unitOfWork.Trip.Get(t => t.Id == booking.TripId);

            return View(booking);
        }

        public ActionResult Details(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(u => u.Id == bookingId);
            booking.Trip = _unitOfWork.Trip.Get(u => u.Id == booking.TripId);
            booking.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == booking.ApplicationUserId);
            booking.Participants = _unitOfWork.Participant.GetAll(u => u.BookingId == booking.Id).ToList();
            var payment = _unitOfWork.Payment.Get(u => u.BookingId == booking.Id);

            ViewBag.Payment = payment;

            return View(booking);
        }

        #region Private methods
        public Booking CalculateBooking(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId);
            booking.Participants = _unitOfWork.Participant.GetAll(p => p.BookingId == bookingId).ToList();
            booking.Trip = _unitOfWork.Trip.Get(t => t.Id == booking.TripId);

            //count number of single supplements
            var numberOfSingleSupplements = booking.Participants.Count(p => p.RoomType == RoomType.Single);

            //calculate single supplement total
            var singleSupplementTotal = booking.Trip.SingleSupplement * numberOfSingleSupplements;

            //calculate total amount
            booking.TotalAmount = booking.Trip.DiscountPricePerPerson * booking.NumberOfParticipants;
            booking.TotalAmount += singleSupplementTotal;

            _unitOfWork.Booking.Update(booking);
            _unitOfWork.Save();
            return booking;

        }

        private Booking CreateNewBooking(int tripId, string userId)
        {
            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            var booking = new Booking();
            var mainParticipant = new Participant();

            // Assign new booking attributes
            booking.ApplicationUserId = applicationUser.Id;
            booking.BookingDate = DateTime.UtcNow;
            booking.Status = BookingStatus.Pending;
            booking.NumberOfParticipants = 1;  // You can later update this if more participants are added
            booking.TripId = tripId;

            // Save the booking first to generate BookingId
            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();  // Save booking so it has a valid BookingId

            // From ApplicationUser, create a main participant for the booking
            mainParticipant.FirstName = applicationUser.FirstName;
            mainParticipant.LastName = applicationUser.LastName;
            mainParticipant.Email = applicationUser.Email;
            mainParticipant.PhoneNumber = applicationUser.PhoneNumber;
            mainParticipant.ApplicationUserId = applicationUser.Id;
            mainParticipant.BookingId = booking.Id;  // Now this BookingId is valid

            // Add the main participant to the database
            _unitOfWork.Participant.Add(mainParticipant);

            // Save the participant
            _unitOfWork.Save();

            return booking;
        }

        private void SendConfirmationEmail(string userId, int bookingId)
        {
            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            var booking = _unitOfWork.Booking.Get(b => b.Id == bookingId);
            booking.Trip = _unitOfWork.Trip.Get(t => t.Id == booking.TripId);
            booking.Participants = _unitOfWork.Participant.GetAll(p => p.BookingId == booking.Id).ToList();

            if (applicationUser != null && booking != null)
            {
                _emailSender.SendEmailAsync(applicationUser.Email, "EasyTrip: Booking Confirmation",
                    "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<meta charset=\"UTF-8\">\r\n<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n<title>Booking Confirmation</title>\r\n<style>\r\nbody {\r\nfont-family: Arial, sans-serif;\r\nmargin: 0;\r\npadding: 0;\r\nbackground-color: #f4f4f4;\r\n}\r\n.container {\r\nwidth: 100%;\r\nmax-width: 600px;\r\nmargin: 20px auto;\r\nbackground: #ffffff;\r\nborder-radius: 10px;\r\noverflow: hidden;\r\nbox-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);\r\n}\r\n.header {\r\nbackground: #0073e6;\r\ncolor: #ffffff;\r\ntext-align: center;\r\npadding: 20px;\r\nfont-size: 24px;\r\nfont-weight: bold;\r\n}\r\n.content {\r\npadding: 20px;\r\ntext-align: center;\r\ncolor: #333;\r\n}\r\n.content h2 {\r\ncolor: #0073e6;\r\n}\r\n.button {\r\ndisplay: inline-block;\r\nbackground: #0073e6;\r\ncolor: #ffffff;\r\npadding: 12px 20px;\r\ntext-decoration: none;\r\nborder-radius: 5px;\r\nfont-size: 16px;\r\nmargin-top: 20px;\r\n}\r\n.footer {\r\nbackground: #f4f4f4;\r\ntext-align: center;\r\npadding: 15px;\r\nfont-size: 14px;\r\ncolor: #666;\r\n}\r\n.footer a {\r\ncolor: #0073e6;\r\ntext-decoration: none;\r\n}\r\n</style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n" +
                    $"<div class=\"header\">\r\nYour Booking is Confirmed! \ud83c\udf89\r\n</div>\r\n\r\n\r\n<div class=\"content\">\r\n<h2>Thank You for Booking with Us!</h2>\r\n<p>Your trip details are confirmed. We are excited to take you on an unforgettable adventure!</p>\r\n<p><strong>Booking ID:</strong> {booking.Id}</p>\r\n<p><strong>Destination:</strong> {booking.Trip.Destination}</p>\r\n<p><strong>Departure Date:</strong> {booking.Trip.StartDate}</p>\r\n<p><strong>Return Date:</strong>{booking.Trip.StartDate}</p>\r\n\r\n</div>\r\n\r\n<div class=\"footer\">\r\n<p>Need Help? <a href=\"mailto:easy.trip@gmail.com\">Contact Us</a></p>\r\n<p>&copy; 2025 Easy Trip. All Rights Reserved.</p>\r\n</div>\r\n</div>\r\n</body>\r\n</html>\r\n");
            }
        }

    }

    #endregion
}

