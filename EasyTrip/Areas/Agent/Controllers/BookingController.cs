using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Areas.Agent.Controllers
{
    [Area(UserRole.Agent)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // GET: All trips
        public ActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var bookings = _unitOfWork.Booking.GetAll(includeProperties: "Trip").ToList();

            if (User.IsInRole(UserRole.Agent))
            {
                bookings = bookings.Where(t => t.ApplicationUserId == userId).ToList();
            }

            return View(bookings);
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

        [HttpPost]
        public IActionResult Delete(int bookingId)
        {
            var booking = _unitOfWork.Booking.Get(u => u.Id == bookingId);

            if (booking == null)
            {
                TempData["error"] = "Booking not found";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.Booking.Remove(booking);
            _unitOfWork.Save();

            TempData["success"] = "Booking deleted successfully";

            return RedirectToAction(nameof(Index));


        }
    }
}
