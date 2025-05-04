using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Agent.Controllers
{
    [Area(UserRole.Agent)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    public class ParticipantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ParticipantController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var participants = _unitOfWork.Participant.GetAll().ToList();
            foreach (var participant in participants)
            {
                participant.Booking = _unitOfWork.Booking.Get(u => u.Id == participant.BookingId);
                participant.Booking.Trip = _unitOfWork.Trip.Get(u => u.Id == participant.Booking.TripId);
            }

            return View(participants);
        }

    }
}
