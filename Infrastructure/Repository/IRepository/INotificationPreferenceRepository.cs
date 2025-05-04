using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface INotificationPreferenceRepository : IRepository<NotificationPreference>
    {
        void Update(NotificationPreference notificationPreference);
    }
}
