using Domain.Enums;
using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        private readonly ApplicationDbContext _db;
        public PaymentRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Payment payment)
        {
            _db.Payments.Update(payment);
        }

        public void UpdateStatus(int id, PaymentStatus paymentStatus)
        {
            var paymentFromDb = _db.Payments.FirstOrDefault(s => s.Id == id);
            if (paymentFromDb != null)
            {
                paymentFromDb.Status = paymentStatus;
            }
        }

        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var paymentFromDb = _db.Payments.FirstOrDefault(s => s.Id == id);

            if (!string.IsNullOrEmpty(sessionId))
            {
                paymentFromDb.SessionId = sessionId;
            }

            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                paymentFromDb.PaymentIntentId = paymentIntentId;
                paymentFromDb.PaymentDate = DateTime.UtcNow;
            }
        }
    }
}
