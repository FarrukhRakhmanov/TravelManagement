using Domain.Models;
using Domain.StaticDetails;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Areas.Admin.API
{
    [Area(UserRole.Admin)]
    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Agent}")]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationsController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;

        }

        [HttpGet("user")]
        public IActionResult GetUserNotifications()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var notificationPreferences = _unitOfWork.NotificationPreference
                .GetAll()
                .Where(p => p.ApplicationUserId == userId && p.InApp == true);

            var notifications = new List<Notification>();

            foreach (var notificationPreference in notificationPreferences)
            {
                var notif = _unitOfWork.Notification.GetAll(n => n.Type == notificationPreference.Type && n.ApplicationUserId == userId);
                if (notif != null && notif.Any())
                {
                    notifications.AddRange(notif);
                }
            }

            notifications = notifications
                .Where(n => n.IsRead == false)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToList();

            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public IActionResult GetNotification(int id)
        {
            var notification = _unitOfWork.Notification.Get(n => n.Id == id);
            if (notification == null) return NotFound();

            return Ok(notification);
        }
    }

}
