using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class NotificationPreferenceRepository : Repository<NotificationPreference>, INotificationPreferenceRepository
    {
        private readonly ApplicationDbContext _db;
        public NotificationPreferenceRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(NotificationPreference notificationPreference)
        {
            _db.NotificationPreferences.Update(notificationPreference);
        }
    }
}
