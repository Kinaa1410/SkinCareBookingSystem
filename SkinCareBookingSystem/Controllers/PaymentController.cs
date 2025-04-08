using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayOSService.Config;
using PayOSService.DTO;
using PayOSService.Services;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.Enums;
using SkinCareBookingSystem.Implements;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Models;
using SkinCareBookingSystem.Util;
using System;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly VNPAYPayment _vnpayPayment;
        private readonly IBookingService _bookingService;
        private readonly BookingDbContext _context;
        private readonly PayOSConfig _payOSConfig;
        private readonly IPayOSService _payOsService;

        public PaymentController(
            IBookingService bookingService,
            PayOSConfig payOsConfig,
            BookingDbContext context,
            IPayOSService payOsService,
            IOptions<PayOSConfig> payOSConfig)
        {
            var tmnCode = "YourMerchantCode";
            var secretKey = "YourSecretKey";
            _bookingService = bookingService;
            _context = context;
            _vnpayPayment = new VNPAYPayment(tmnCode, secretKey, context);
            _payOsService = payOsService;
            _payOSConfig = payOSConfig.Value;
        }

        [HttpPost("create-payos-payment")]
        public async Task<IActionResult> CreatePayment(int bookingID)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.TherapistTimeSlot)
                    .FirstOrDefaultAsync(x => x.BookingId == bookingID);
                if (booking == null) return NotFound();

                if (booking.Status != BookingStatus.Pending)
                    return BadRequest("Booking is not in a pending state.");

                var existingLock = await _context.TherapistTimeSlotLocks
                    .FirstOrDefaultAsync(tsl => tsl.TherapistTimeSlotId == booking.TherapistTimeSlotId &&
                                                tsl.Date == booking.AppointmentDate.Date &&
                                                (tsl.Status == SlotStatus.InProcess || tsl.Status == SlotStatus.Booked));
                if (existingLock != null)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = "Time slot is already in process or booked for this date." });
                }

                var timeSlotLock = new TherapistTimeSlotLock
                {
                    TherapistTimeSlotId = booking.TherapistTimeSlotId,
                    Date = booking.AppointmentDate.Date,
                    Status = SlotStatus.InProcess
                };
                _context.TherapistTimeSlotLocks.Add(timeSlotLock);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Created TherapistTimeSlotLock: Id={timeSlotLock.Id}, TherapistTimeSlotId={booking.TherapistTimeSlotId}, Date={booking.AppointmentDate.Date}, Status=InProcess for BookingId={bookingID}");

                var existingTransaction = await _context.Transactions
                    .FirstOrDefaultAsync(x => x.BookingID == bookingID);

                if (existingTransaction != null && !string.IsNullOrEmpty(existingTransaction.PaymentLink))
                {
                    await transaction.CommitAsync();
                    Console.WriteLine($"Returning existing payment link for BookingId: {bookingID}, Status: {booking.Status}, PaymentLink: {existingTransaction.PaymentLink}");
                    return Ok(new { paymentLink = existingTransaction.PaymentLink });
                }

                var code = ApplicationUtil.GetNewID();
                var paymentLink = await _payOsService.CreatePaymentAsync(new CreatePaymentDTO
                {
                    OrderCode = code,
                    Content = "Thanh toan",
                    RequiredAmount = (int)booking.TotalPrice
                });

                await _vnpayPayment.AddTransactionAsync(code, booking.BookingId, paymentLink, (decimal)booking.TotalPrice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"Payment link created for BookingId: {bookingID}, Status: {booking.Status}, OrderCode: {code}, PaymentLink: {paymentLink}, ReturnUrl: {_payOSConfig.ReturnUrl}");
                return Ok(new { paymentLink });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"CreatePayment Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { message = $"Error creating payment: {ex.Message}" });
            }
        }

        [HttpGet("payment-return")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentReturn(
            [FromQuery] string code,
            [FromQuery] string id,
            [FromQuery] string cancel,
            [FromQuery] string status,
            [FromQuery] string orderCode)
        {
            var queryParams = Request.QueryString.Value;
            Console.WriteLine($"PaymentReturn called with query params: {queryParams}");

            if (!long.TryParse(orderCode, out long orderId))
            {
                Console.WriteLine($"Invalid orderCode: {orderCode}");
                return BadRequest(new { success = false, message = "Invalid orderCode." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var transactionRecord = await _context.Transactions
                    .Include(t => t.Booking)
                    .ThenInclude(b => b.TherapistTimeSlot)
                    .FirstOrDefaultAsync(x => x.ID == orderId);
                if (transactionRecord == null)
                {
                    Console.WriteLine($"Transaction not found for orderId: {orderId}");
                    return BadRequest(new { success = false, message = "Transaction not found." });
                }

                var booking = transactionRecord.Booking;
                if (booking == null || booking.TherapistTimeSlot == null)
                {
                    Console.WriteLine($"Booking or TherapistTimeSlot not found for transaction orderId: {orderId}");
                    return BadRequest(new { success = false, message = "Booking or TherapistTimeSlot not found." });
                }

                var timeSlotLock = await _context.TherapistTimeSlotLocks
                    .FirstOrDefaultAsync(tsl => tsl.TherapistTimeSlotId == booking.TherapistTimeSlotId &&
                                                tsl.Date == booking.AppointmentDate.Date);
                Console.WriteLine($"PaymentReturn: TimeSlotLock state on entry: {(timeSlotLock != null ? $"Id={timeSlotLock.Id}, Status={timeSlotLock.Status}" : "Not found - possibly deleted externally")} for BookingId={booking.BookingId}");

                if (timeSlotLock == null)
                {
                    Console.WriteLine($"PaymentReturn: No lock found for BookingId: {booking.BookingId} - may have been deleted prematurely.");
                }

                Console.WriteLine($"PaymentReturn: orderId={orderId}, code={code}, status={status}, cancel={cancel}");

                if (code == "00" && status?.ToUpper() == "PAID")
                {
                    booking.IsPaid = true;
                    booking.Status = BookingStatus.Booked;
                    if (timeSlotLock != null)
                    {
                        timeSlotLock.Status = SlotStatus.Booked;
                        _context.Update(timeSlotLock);
                        Console.WriteLine($"PaymentReturn: Payment succeeded - Booking {booking.BookingId} set to Booked, TimeSlotLock {timeSlotLock.Id} set to Booked.");
                    }
                    else
                    {
                        // Recreate lock if missing
                        var newLock = new TherapistTimeSlotLock
                        {
                            TherapistTimeSlotId = booking.TherapistTimeSlotId,
                            Date = booking.AppointmentDate.Date,
                            Status = SlotStatus.Booked
                        };
                        _context.TherapistTimeSlotLocks.Add(newLock);
                        Console.WriteLine($"PaymentReturn: Payment succeeded - Booking {booking.BookingId} set to Booked, Recreated TimeSlotLock for TherapistTimeSlotId={booking.TherapistTimeSlotId}.");
                    }
                }
                else if (status?.ToUpper() == "CANCELLED" && (cancel?.ToLower() == "true" || cancel == "1"))
                {
                    //var timeSinceTransaction = DateTime.Now - transactionRecord.Date;
                    //if (timeSinceTransaction.TotalMinutes < 1)
                    //{
                    //    Console.WriteLine($"PaymentReturn: Immediate cancellation - Booking {booking.BookingId} not updated.");
                    //}
                    //else
                    //{
                        booking.IsPaid = false;
                        booking.Status = BookingStatus.Failed;
                        if (timeSlotLock != null)
                        {
                            _context.TherapistTimeSlotLocks.Remove(timeSlotLock);
                            Console.WriteLine($"PaymentReturn: Payment cancelled - Booking {booking.BookingId} set to Failed, TimeSlotLock {timeSlotLock.Id} removed.");
                        }
                    }
                //}
                else
                {
                    Console.WriteLine($"PaymentReturn: Unknown status - Booking {booking.BookingId} not updated. Code: {code}, Status: {status}, Cancel: {cancel}");
                }

                _context.Entry(booking).Property(b => b.Status).IsModified = true;
                _context.Entry(booking).Property(b => b.IsPaid).IsModified = true;

                Console.WriteLine($"PaymentReturn: Before Save - BookingId={booking.BookingId}, Status={booking.Status}, IsPaid={booking.IsPaid}, TimeSlotLock Status={timeSlotLock?.Status.ToString() ?? "None"}");
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"PaymentReturn: Processed - BookingId={booking.BookingId}, Status={booking.Status}, IsPaid={booking.IsPaid}, TimeSlotLock Status={timeSlotLock?.Status.ToString() ?? "None"}");
                return Ok(new { success = true, redirectUrl = _payOSConfig.ClientRedirectUrl });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"PaymentReturn Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Error processing payment return" });
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RedirectToVNPAY(string orderRef, string amount)
        {
            var paymentUrl = _vnpayPayment.BuildPaymentUrl(orderRef, amount);
            return Redirect(paymentUrl);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ReturnUrl()
        {
            var response = Request.Query;
            string secureHash = response["vnp_SecureHash"];
            if (VerifySecureHash(response, secureHash))
            {
                string paymentStatus = response["vnp_ResponseCode"];
                if (paymentStatus == "00")
                {
                    return View("PaymentSuccess");
                }
                else
                {
                    return View("PaymentFailure");
                }
            }
            return View("Error");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private bool VerifySecureHash(Microsoft.AspNetCore.Http.IQueryCollection response, string secureHash)
        {
            var parameters = new System.Collections.Specialized.NameValueCollection();
            foreach (var key in response.Keys)
            {
                parameters.Add(key, response[key]);
            }
            var calculatedHash = _vnpayPayment.GenerateSecureHash(parameters);
            return calculatedHash == secureHash;
        }

        [HttpGet]
        [Route("api/payment/generate-url")]
        public IActionResult GeneratePaymentUrl(string orderRef, string amount)
        {
            var paymentUrl = _vnpayPayment.BuildPaymentUrl(orderRef, amount);
            return Ok(new { PaymentUrl = paymentUrl });
        }
    }
}