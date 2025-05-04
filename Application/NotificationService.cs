using Domain.Enums;
using Domain.Models;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;

namespace Application
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext,
            IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task CreateAndSendNotificationAsync(string userId, string message, string title,
            NotificationType type)
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            List<Notification> allNotifications = new List<Notification>();

            foreach (var admin in admins)
            {
                var notif = new Notification
                {
                    Message = message,
                    Title = title,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Type = type,
                    ApplicationUserId = admin.Id,
                };
                allNotifications.Add(notif);
                _unitOfWork.Notification.Add(notif);
            }

            if (!admins.Contains(user))
            {
                var userNotification = new Notification
                {
                    Message = message,
                    Title = title,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false,
                    Type = type,
                    ApplicationUserId = userId,
                };

                allNotifications.Add(userNotification);

                _unitOfWork.Notification.Add(userNotification);
            }

            _unitOfWork.Save();

            await SendNotificationWithPreferences(allNotifications);
        }

        public async Task SendNotificationWithPreferences(List<Notification> notifications)
        {
            var allPreferences = _unitOfWork.NotificationPreference
                .GetAll(includeProperties: "ApplicationUser")
                .Where(p => notifications.Select(n => n.ApplicationUserId).Contains(p.ApplicationUserId))
                .ToList();

            var groupedPreferences = allPreferences
                .GroupBy(p => p.ApplicationUserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var notif in notifications)
            {
                if (groupedPreferences.TryGetValue(notif.ApplicationUserId, out var userPreferences))
                {
                    foreach (var pref in userPreferences)
                    {
                        if (pref.Type == notif.Type)
                        {
                            if (pref.InApp)
                            {
                                await _hubContext.Clients.User(pref.ApplicationUserId)
                                    .SendAsync("ReceiveNotification", notif);
                            }

                            if (pref.ByEmail)
                            {
                                await _emailSender.SendEmailAsync(pref.ApplicationUser.Email, notif.Title,
                                    notif.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}
