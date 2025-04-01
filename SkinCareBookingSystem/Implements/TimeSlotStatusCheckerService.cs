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
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _timeoutInterval = TimeSpan.FromMinutes(5);

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

                _logger.LogInformation($"Found {timeSlots.Count} InProcess time slots.");

                foreach (var timeSlot in timeSlots)
                {
                    var bookings = await context.Bookings
                        .Where(b => b.TimeSlotId == timeSlot.Id && b.AppointmentDate.Date >= DateTime.Now.Date)
                        .ToListAsync();

                    _logger.LogInformation($"TimeSlot {timeSlot.Id}: Found {bookings.Count} bookings.");

                    foreach (var booking in bookings)
                    {
                        // Skip if booking is already in a final state (Booked or Failed)
                        if (booking.Status == BookingStatus.Booked || booking.Status == BookingStatus.Failed)
                        {
                            _logger.LogInformation($"Booking {booking.BookingId} already in final state ({booking.Status}). Skipping.");
                            continue;
                        }

                        _logger.LogInformation($"Processing Booking {booking.BookingId}, Status: {booking.Status}, IsPaid: {booking.IsPaid}, DateCreated: {booking.DateCreated}");

                        var transactionRecord = await context.Transactions
                            .FirstOrDefaultAsync(t => t.BookingID == booking.BookingId);

                        if (transactionRecord == null)
                        {
                            _logger.LogWarning($"No transaction found for Booking {booking.BookingId}. Using DateCreated for timeout check.");
                            var timeSinceCreation = DateTime.Now - booking.DateCreated;
                            if (timeSinceCreation < _timeoutInterval)
                                continue;

                            if (!booking.IsPaid)
                            {
                                booking.Status = BookingStatus.Failed;
                                timeSlot.Status = SlotStatus.Available;
                                _logger.LogInformation($"TimeSlot {timeSlot.Id}, Booking {booking.BookingId} timed out (no transaction). Set to Available and Failed.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"Transaction found for Booking {booking.BookingId}, Date: {transactionRecord.Date}");
                            var timeSinceTransaction = DateTime.Now - transactionRecord.Date;

                            if (timeSinceTransaction >= _timeoutInterval)
                            {
                                if (!booking.IsPaid)
                                {
                                    booking.Status = BookingStatus.Failed;
                                    timeSlot.Status = SlotStatus.Available;
                                    _logger.LogInformation($"TimeSlot {timeSlot.Id}, Booking {booking.BookingId} timed out. Set to Available and Failed.");
                                }
                            }
                            else
                            {
                                _logger.LogInformation($"Booking {booking.BookingId} still within timeout period. Skipping.");
                                continue;
                            }
                        }

                        context.Entry(booking).Property(b => b.Status).IsModified = true;
                        context.Entry(timeSlot).Property(ts => ts.Status).IsModified = true;
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