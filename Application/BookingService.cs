using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application
{
    public class BookingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ILogger<BookingService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BookingService is starting.");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var finishedTripIds = await dbContext.Trips
                        .Where(t => t.Status == TripStatus.Finished)
                        .Select(t => t.Id)
                        .ToListAsync();

                    var bookings = await dbContext.Bookings
                        .Where(b => b.Status == BookingStatus.Confirmed && finishedTripIds.Contains(b.TripId))
                        .ToListAsync();

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        foreach (var booking in bookings)
                        {
                            booking.Status = BookingStatus.Completed;
                            dbContext.Bookings.Update(booking);
                            _logger.LogInformation($"Booking {booking.Id} status changed to Completed.");
                        }

                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while updating booking statuses.");
                        await transaction.RollbackAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }

    }
}
