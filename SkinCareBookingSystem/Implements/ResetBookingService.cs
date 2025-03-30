using SkinCareBookingSystem.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SkinCareBookingSystem.Implements
{
    public class ResetBookingService : BackgroundService
    {
        private readonly IServiceProvider _services;
        public ResetBookingService(IServiceProvider services) => _services = services;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<ITherapistScheduleService>();
                await service.ResetCompletedBookingsAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}