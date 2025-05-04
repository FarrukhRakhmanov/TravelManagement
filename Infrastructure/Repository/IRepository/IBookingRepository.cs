using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void Update(Booking booking);
    }
}
