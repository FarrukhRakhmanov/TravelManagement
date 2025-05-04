using Domain.ViewModels;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class ParticipantController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IActionResult Add(BookingVM bookingVM)
        {
            if (bookingVM.Participant == null || bookingVM.Booking == null)
            {
                TempData["error"] = "Invalid booking or participant data";
                return RedirectToAction("Details", "Booking", new { bookingId = bookingVM.Participant.BookingId });
            }

            bookingVM.Participant.BookingId = bookingVM.Booking.Id;
            var booking = _unitOfWork.Booking.Get(u => u.Id == bookingVM.Booking.Id);
            var trip = _unitOfWork.Trip.Get(u => u.Id == booking.TripId);
            var numberOfParticipants = booking.NumberOfParticipants + 1;

            if (numberOfParticipants > trip.SeatsAvailable)
            {
                TempData["error"] = "Not enough seats available to add more participants";

                return RedirectToAction("Initiate", "Booking", new { bookingId = bookingVM.Participant.BookingId });
            }

            if (ModelState.IsValid)
            {
                if (bookingVM.Participant.Id == 0)
                {
                    _unitOfWork.Participant.Add(bookingVM.Participant);
                    booking.NumberOfParticipants++;
                    TempData["success"] = "Participant added successfully";
                }
                else
                {
                    _unitOfWork.Participant.Update(bookingVM.Participant);
                    TempData["success"] = "Participant updated successfully";
                }

                _unitOfWork.Booking.Update(booking);
                _unitOfWork.Save();

                return RedirectToAction("Initiate", "Booking", new { bookingId = bookingVM.Participant.BookingId });
            }

            TempData["error"] = "Participant not added";
            return RedirectToAction("Initiate", "Booking", new { bookingId = bookingVM.Participant.BookingId });
        }

        [HttpPost]
        public IActionResult Delete(int participantId)
        {
            var participant = _unitOfWork.Participant.Get(p => p.Id == participantId);

            if (participant == null)
            {
                TempData["error"] = "Participant not found";
                return RedirectToAction("Index", "Booking", new { bookingId = participant.BookingId });
            }

            var booking = _unitOfWork.Booking.Get(b => b.Id == participant.BookingId);

            if (booking != null && booking.NumberOfParticipants > 0)
            {
                booking.NumberOfParticipants--;
                _unitOfWork.Booking.Update(booking);
            }

            _unitOfWork.Participant.Remove(participant);
            _unitOfWork.Save();

            TempData["success"] = "Participant deleted successfully";
            return RedirectToAction("Details", "Booking", new { bookingId = participant.BookingId });
        }
    }
}
