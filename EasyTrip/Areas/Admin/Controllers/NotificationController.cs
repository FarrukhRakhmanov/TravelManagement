using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Areas.Admin.Controllers
{
    [Area(UserRole.Admin)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]

    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var notifications = _unitOfWork.Notification.GetAll(n => n.ApplicationUserId == userId).ToList();

            return View(notifications);
        }

        [HttpGet]
        public IActionResult Details(int notificationId)
        {
            var notification = _unitOfWork.Notification.Get(n => n.Id == notificationId);
            if (notification == null) return NotFound();

            notification.IsRead = true;

            _unitOfWork.Notification.Update(notification);
            _unitOfWork.Save();

            return View(notification);
        }

        [HttpPost]
        public IActionResult Delete(int notificationId)
        {
            var notification = _unitOfWork.Notification.Get(n => n.Id == notificationId);
            if (notification == null) return NotFound();

            _unitOfWork.Notification.Remove(notification);
            _unitOfWork.Save();

            TempData["success"] = "Notification deleted successfully";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult NotificationPreferences()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var notificationPreferences = _unitOfWork.NotificationPreference
                .GetAll(p => p.ApplicationUserId == userId).ToList();

            return View(notificationPreferences);
        }

        [HttpPost]
        public IActionResult UpdateNotificationPreferences(List<NotificationPreference> notificationPreferences)
        {
            foreach (var pref in notificationPreferences)
            {
                var existingPref = _unitOfWork.NotificationPreference.Get(x => x.Id == pref.Id);

                if (existingPref != null)
                {
                    existingPref.ByEmail = pref.ByEmail;
                    existingPref.InApp = pref.InApp;
                    _unitOfWork.NotificationPreference.Update(existingPref);
                }
            }

            _unitOfWork.Save();

            TempData["success"] = "Notification preferences updated successfully";
            return RedirectToAction(nameof(NotificationPreferences));
        }

    }
}
