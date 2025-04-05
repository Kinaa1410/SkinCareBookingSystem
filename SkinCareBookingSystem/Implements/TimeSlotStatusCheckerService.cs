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
                    await CheckAndUpdateTimeSlotLocksAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking TimeSlotLock statuses.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("TimeSlotStatusCheckerService stopped.");
        }

        private async Task CheckAndUpdateTimeSlotLocksAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var timeSlotLocks = await context.TherapistTimeSlotLocks
                    .Where(tsl => tsl.Status == SlotStatus.InProcess)
                    .ToListAsync();

                _logger.LogInformation($"Found {timeSlotLocks.Count} InProcess time slot locks.");

                foreach (var timeSlotLock in timeSlotLocks)
                {
                    var booking = await context.Bookings
                        .FirstOrDefaultAsync(b => b.TherapistTimeSlotId == timeSlotLock.TherapistTimeSlotId &&
                                                  b.AppointmentDate.Date == timeSlotLock.Date);

                    if (booking == null)
                    {
                        _logger.LogWarning($"No booking found for TimeSlotLock {timeSlotLock.Id}. Removing stale lock.");
                        context.TherapistTimeSlotLocks.Remove(timeSlotLock);
                        continue;
                    }
                    if (booking.Status == BookingStatus.Booked || booking.Status == BookingStatus.Failed)
                    {
                        _logger.LogInformation($"Booking {booking.BookingId} already in final state ({booking.Status}). Removing lock.");
                        context.TherapistTimeSlotLocks.Remove(timeSlotLock);
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
                            context.TherapistTimeSlotLocks.Remove(timeSlotLock);
                            _logger.LogInformation($"TimeSlotLock {timeSlotLock.Id}, Booking {booking.BookingId} timed out (no transaction). Removed lock and set booking to Failed.");
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
                                context.TherapistTimeSlotLocks.Remove(timeSlotLock);
                                _logger.LogInformation($"TimeSlotLock {timeSlotLock.Id}, Booking {booking.BookingId} timed out. Removed lock and set booking to Failed.");
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"Booking {booking.BookingId} still within timeout period. Skipping.");
                            continue;
                        }
                    }

                    context.Entry(booking).Property(b => b.Status).IsModified = true;
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while updating TimeSlotLock statuses.");
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