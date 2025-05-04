using Infrastructure.Data;
using Infrastructure.Repository.IRepository;
using Infrastructure.Repository;

namespace Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IItineraryRepository Itinerary { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public ITripRepository Trip { get; private set; }
        public IParticipantRepository Participant { get; private set; }
        public IBookingRepository Booking { get; private set; }

        public IStripeSettingsRepository StripeSettings { get; private set; }

        public IPaymentRepository Payment { get; private set; }

        public INotificationRepository Notification { get; private set; }

        public IReviewRepository Review { get; private set; }

        public IContactMessageRepository ContactMessage { get; private set; }

        public INotificationPreferenceRepository NotificationPreference { get; private set; }



        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ApplicationUser = new ApplicationUserRepository(_db);
            Itinerary = new ItineraryRepository(_db);
            Trip = new TripRepository(_db);
            Participant = new ParticipantRepository(_db);
            Booking = new BookingRepository(_db);
            StripeSettings = new StripeSettingsRepository(_db);
            Payment = new PaymentRepository(_db);
            Notification = new NotificationRepository(_db);
            Review = new ReviewRepository(_db);
            ContactMessage = new ContactMessageRepository(_db);
            Notification = new NotificationRepository(_db);
            NotificationPreference = new NotificationPreferenceRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
