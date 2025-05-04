using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class ContactMessageRepository : Repository<ContactMessage>, IContactMessageRepository
    {
        private readonly ApplicationDbContext _db;

        public ContactMessageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ContactMessage cm)
        {
            _db.ContactMessages.Update(cm);
        }
    }
}
