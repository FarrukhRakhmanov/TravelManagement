using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application
{
    public class TripReviewService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TripReviewService> _logger;

        public TripReviewService(ILogger<TripReviewService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("FeedbackService is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                        var trips = dbContext.Trips.Where(t => t.Status == TripStatus.Finished && t.HasReviewSent == false).ToList();

                        foreach (var trip in trips)
                        {
                            var bookings = dbContext.Bookings
                                .Where(b => b.TripId == trip.Id && b.Status == BookingStatus.Completed).ToList();

                            var participants = dbContext.Participants
                                .Where(p => bookings.Select(b => b.Id).Contains(p.BookingId)).ToList();

                            foreach (var participant in participants)
                            {
                                if (participant != null)
                                {
                                    await emailSender.SendEmailAsync(participant.Email, "EasyTrip: Trip Review Request",
                                        $"<h2>Welcome back {participant.FirstName}</h2>\n\n\n" +
                                        $"<p>We hope you enjoyed your recent trip with us. " +
                                        $"We would be happy to read from you about your experience.\n\n\n" +
                                        $"</p> Please leave us feedback through the link below:\n" +
                                        $"<a href=\"https://localhost:7124/User/Review/SubmitReview?participantId={participant.Id}&tripId={trip.Id}\">Leave a Review</a>" +
                                        $"<p>Thanks in advance.</p>" +
                                        $"<p>With best regards,</p><p>EasyTrip Team</p>");
                                }
                            }

                            trip.HasReviewSent = true;
                            dbContext.Trips.Update(trip);
                            _logger.LogInformation($"Review sent for trip ID {trip.Id}.");
                        }

                        await dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing feedback emails.");
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }
    }
}
