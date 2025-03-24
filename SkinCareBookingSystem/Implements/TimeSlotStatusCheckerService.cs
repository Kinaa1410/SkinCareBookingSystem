using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BackgroundServices
{
    public class TimeSlotStatusCheckerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TimeSlotStatusCheckerService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _timeoutInterval = TimeSpan.FromMinutes(10);

        public TimeSlotStatusCheckerService(IServiceProvider serviceProvider, ILogger<TimeSlotStatusCheckerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TimeSlotStatusCheckerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdateTimeSlotsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking TimeSlot statuses.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("TimeSlotStatusCheckerService stopped.");
        }

        private async Task CheckAndUpdateTimeSlotsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var timeSlots = await context.TherapistTimeSlots
                    .Where(ts => ts.Status == SlotStatus.InProcess)
                    .ToListAsync();

                foreach (var timeSlot in timeSlots)
                {
                    var booking = await context.Bookings
                        .FirstOrDefaultAsync(b => b.TimeSlotId == timeSlot.Id && b.AppointmentDate.Date >= DateTime.Now.Date);

                    if (booking == null) continue;

                    var transactionRecord = await context.Transactions
                        .FirstOrDefaultAsync(t => t.BookingID == booking.BookingId);

                    if (transactionRecord == null) continue;

                    var timeSinceTransaction = DateTime.Now - transactionRecord.Date;
                    if (timeSinceTransaction < _timeoutInterval)
                        continue;

                    if (!booking.IsPaid)
                    {
                        timeSlot.Status = SlotStatus.Available;
                        booking.Status = false;
                        context.Bookings.Update(booking);
                        _logger.LogInformation($"TimeSlot {timeSlot.TimeSlotId} has been in InProcess for over 10 minutes without payment. Set to Available.");
                    }
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating TimeSlot statuses.");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TimeSlotStatusCheckerService is stopping.");
            await base.StopAsync(cancellationToken);
        }
    }
}