using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IStripeSettingsRepository : IRepository<StripeSettings>
    {
        void Update(StripeSettings stripeSettings);
    }
}
