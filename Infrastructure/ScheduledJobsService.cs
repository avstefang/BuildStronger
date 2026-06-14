using Application.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class ScheduledJobsService(IServiceScopeFactory scopeFactory, ILogger<ScheduledJobsService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                using IServiceScope scope = scopeFactory.CreateScope();

                var subscriptionService = scope.ServiceProvider.GetRequiredService<SubscriptionService>();
                await subscriptionService.ActivateLatentSubscriptionAsync();
                logger.LogInformation("Activated latent subscriptions at {Time}", DateTime.UtcNow);

                var reservationService = scope.ServiceProvider.GetRequiredService<ReservationService>();
                await reservationService.AutoAcceptWaitlistReservationsAsync();
                logger.LogInformation("Processed reservation waitlist at {Time}", DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Scheduled job failed at {Time}", DateTime.UtcNow);
            }
        }
    }
}
