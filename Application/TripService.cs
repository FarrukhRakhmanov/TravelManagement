using Domain.Enums;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Application
{
    public class TripService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TripService> _logger;

        public TripService(ILogger<TripService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TripService is starting.");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

                    var trips = dbContext.Trips.Where(t => t.Status == TripStatus.Published || t.Status == TripStatus.Sold || t.Status == TripStatus.Ongoing).ToList();

                    using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        foreach (var trip in trips)
                        {
                            if (trip.StartDate == today && (trip.Status == TripStatus.Published || trip.Status == TripStatus.Sold))
                            {
                                trip.Status = TripStatus.Ongoing;
                                dbContext.Trips.Update(trip);
                                _logger.LogInformation("Trip status changed to Ongoing.");
                            }

                            if (trip.EndDate < today && trip.Status == TripStatus.Ongoing)
                            {
                                trip.Status = TripStatus.Finished;
                                dbContext.Trips.Update(trip);
                                _logger.LogInformation("Trip status changed to Finished.");
                            }
                        }

                        await dbContext.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while updating trip statuses.");
                        await transaction.RollbackAsync();
                    }
                }

                await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
            }
        }

    }
}
