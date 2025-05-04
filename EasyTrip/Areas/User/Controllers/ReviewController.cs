using Application;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.User.Controllers
{
    [Area(nameof(User))]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly NotificationService _notificationService;

        public ReviewController(IUnitOfWork unitOfWork, NotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult SubmitReview(int participantId, int tripId)
        {
            var reviewsFromDb = _unitOfWork.Review.GetAll(u => u.ParticipantId == participantId && u.TripId == tripId).ToList();

            if (reviewsFromDb.Count() > 0)
            {
                TempData["error"] = "You have already left a review for this trip. Thank you again!";
                return RedirectToAction("ThankYou", new { participantId });
            }

            var review = new Review
            {
                ParticipantId = participantId,
                TripId = tripId
            };
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReview(Review review)
        {
            review.CreatedAt = DateTime.UtcNow;
            review.Status = ReviewStatus.Pending;

            var trip = _unitOfWork.Trip.Get(t => t.Id == review.TripId);
            var participant = _unitOfWork.Participant.Get(p => p.Id == review.ParticipantId);


            if (trip == null || participant == null)
            {
                TempData["error"] = "Trip or participant not found.";
                return View(review);
            }

            var applicationUser = _unitOfWork.ApplicationUser.Get(t => t.Id == trip.ApplicationUserId);

            if (applicationUser == null)
            {
                TempData["error"] = "Application user not found.";
                return View(review);
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Review.Add(review);
                _unitOfWork.Save();

                await _notificationService.CreateAndSendNotificationAsync(applicationUser.Id,
                    $"Review by: {participant.FullName()} - submitted",
                    "New review submission", NotificationType.Review);

                return RedirectToAction("ThankYou", new { participantId = review.ParticipantId });
            }

            return View(review);

        }

        public IActionResult ThankYou(int participantId)
        {
            var participant = _unitOfWork.Participant.Get(u => u.Id == participantId);

            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }
    }
}
