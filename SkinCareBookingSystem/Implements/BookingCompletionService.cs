using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BackgroundServices
{
    public class BookingCompletionService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BookingCompletionService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1); 
        public BookingCompletionService(IServiceProvider serviceProvider, ILogger<BookingCompletionService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BookingCompletionService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndCompleteBookingsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking and completing bookings.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("BookingCompletionService stopped.");
        }

        private async Task CheckAndCompleteBookingsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var completedBookings = await context.Bookings
                    .Where(b => b.Status == BookingStatus.Booked && b.AppointmentDate < DateTime.Now)
                    .ToListAsync();

                _logger.LogInformation($"Found {completedBookings.Count} bookings to complete.");

                foreach (var booking in completedBookings)
                {
                    var timeSlot = await context.TherapistTimeSlots
                        .FirstOrDefaultAsync(ts => ts.Id == booking.TimeSlotId);

                    if (timeSlot != null)
                    {
                        timeSlot.Status = SlotStatus.Available;
                        booking.Status = BookingStatus.Completed;

                        context.Entry(timeSlot).State = EntityState.Modified;
                        context.Entry(booking).State = EntityState.Modified;

                        _logger.LogInformation($"Booking {booking.BookingId} completed, TimeSlot {timeSlot.Id} set to Available.");
                    }
                    else
                    {
                        _logger.LogWarning($"TimeSlot for Booking {booking.BookingId} not found.");
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error during booking completion.");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BookingCompletionService is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}