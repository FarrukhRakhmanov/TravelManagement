using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface ITripRepository : IRepository<Trip>
    {
        void Update(Trip trip);
    }
}
