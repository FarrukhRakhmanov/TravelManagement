using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        void Update(Notification notification);
    }
}
