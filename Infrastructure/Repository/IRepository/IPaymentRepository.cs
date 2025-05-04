using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repository.IRepository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        void Update(Payment payment);
        void UpdateStatus(int id, PaymentStatus paymentStatus);
        void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
    }
}
