using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class ItineraryRepository : Repository<Itinerary>, IItineraryRepository
    {
        private readonly ApplicationDbContext _db;
        public ItineraryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Itinerary itinerary)
        {
            _db.Itineraries.Update(itinerary);
        }
    }
}
