using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IItineraryRepository : IRepository<Itinerary>
    {
        void Update(Itinerary itinerary);
    }
}
