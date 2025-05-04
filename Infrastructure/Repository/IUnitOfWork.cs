using Infrastructure.Repository.IRepository;

namespace Infrastructure.Repository
{
    public interface IUnitOfWork
    {
        IApplicationUserRepository ApplicationUser { get; }
        IItineraryRepository Itinerary { get; }

        IParticipantRepository Participant { get; }
        ITripRepository Trip { get; }
        IBookingRepository Booking { get; }

        IStripeSettingsRepository StripeSettings { get; }

        IPaymentRepository Payment { get; }

        IReviewRepository Review { get; }

        IContactMessageRepository ContactMessage { get; }
        INotificationPreferenceRepository NotificationPreference { get; }
        INotificationRepository Notification { get; }

        void Save();
    }

}
