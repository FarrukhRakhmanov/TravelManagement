using Domain.Enums;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Areas.Agent.Controllers
{
    [Area(UserRole.Agent)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var reviews = _unitOfWork.Review.GetAll(includeProperties: "Trip").ToList();

            foreach (var review in reviews)
            {
                review.Participant = _unitOfWork.Participant.Get(p => p.Id == review.ParticipantId);
                review.Trip = _unitOfWork.Trip.Get(t => t.Id == review.TripId);
            }
            if (User.IsInRole(UserRole.Agent))
            {
                reviews = reviews.Where(t => t.Trip.ApplicationUserId == userId).ToList();
            }

            return View(reviews);
        }

        public IActionResult PublishReview(int reviewId)
        {
            var review = _unitOfWork.Review.Get(r => r.Id == reviewId);

            if (review == null)
            {
                return NotFound();
            }

            review.Status = ReviewStatus.Published;
            _unitOfWork.Review.Update(review);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RejectReview(int reviewId)
        {
            var review = _unitOfWork.Review.Get(r => r.Id == reviewId);

            if (review == null)
            {
                return NotFound();
            }

            review.Status = ReviewStatus.Rejected;

            return RedirectToAction(nameof(Index));
        }
    }
}
