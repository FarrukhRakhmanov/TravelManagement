using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class TripRepository : Repository<Trip>, ITripRepository
    {
        private readonly ApplicationDbContext _db;
        public TripRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Trip trip)
        {
            _db.Trips.Update(trip);
        }
    }
}
