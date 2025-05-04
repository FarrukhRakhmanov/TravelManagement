using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IReviewRepository : IRepository<Review>
    {
        void Update(Review review);
    }
}
