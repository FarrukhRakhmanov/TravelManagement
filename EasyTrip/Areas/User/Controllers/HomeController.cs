using Application;
using Domain.Enums;
using Domain.Models;
using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, NotificationService notificationService, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var trips = _unitOfWork.Trip.GetAll(
                t => t.Status == TripStatus.Published
                    && t.StartDate > DateOnly.FromDateTime(DateTime.UtcNow) && t.IsFeatured == true && t.SeatsAvailable > 0).ToList();

            return View(trips);
        }

        [HttpGet]
        public IActionResult Details(int tripId)
        {
            var trip = _unitOfWork.Trip.Get(t => t.Id == tripId);

            if (trip == null)
            {
                return NotFound();
            }

            trip.Itineraries = _unitOfWork.Itinerary
                .GetAll(i => i.TripId == tripId)
                .OrderBy(i => i.Order)
                .ToList();


            var reviews = _unitOfWork.Review.GetAll(r => r.TripId == tripId
                                                         && r.Status == ReviewStatus.Published).ToList();

            foreach (var review in reviews)
            {
                review.Participant = _unitOfWork.Participant.Get(p => p.Id == review.ParticipantId);
            }

            TripVM tripVM = new TripVM()
            {
                Trip = trip,
                Itinerary = new Itinerary(),
                Booking = new Booking(),
                ReviewList = reviews
            };

            return View("TripDetails", tripVM);
        }

        public IActionResult GetAll()
        {
            var trips = _unitOfWork.Trip.GetAll(
                t => (t.Status == TripStatus.Published || t.Status == TripStatus.Sold)
                     && t.StartDate > DateOnly.FromDateTime(DateTime.UtcNow)).ToList();

            return View("AllTrips", trips);
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View(new ContactMessageVM());
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactMessageVM message)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");

            if (ModelState.IsValid)
            {
                _unitOfWork.ContactMessage.Add(message.ContactMessage);
                _unitOfWork.Save();
                ModelState.Clear();

                TempData["Success"] = "Thank you for contacting us. We'll get back to you asap.";

                if (admins.Any())
                {
                    foreach (var admin in admins)
                    {
                        await _notificationService.CreateAndSendNotificationAsync(admin.Id, $"Contact message received from: {message.ContactMessage.FullName()} - \n {message.ContactMessage.Message}",
                            "New contact message", NotificationType.Message);
                    }
                }
                return View();
            }

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
