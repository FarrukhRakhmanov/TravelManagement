using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IContactMessageRepository : IRepository<ContactMessage>
    {
        void Update(ContactMessage contactMessage);
    }
}
