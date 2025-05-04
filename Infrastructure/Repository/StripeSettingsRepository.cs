using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class StripeSettingsRepository : Repository<StripeSettings>, IStripeSettingsRepository
    {
        private readonly ApplicationDbContext _db;
        public StripeSettingsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(StripeSettings stripeSettings)
        {
            _db.StripeSettings.Update(stripeSettings);
        }
    }
}
