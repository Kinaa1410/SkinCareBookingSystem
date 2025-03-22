using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PayOSService.Config;
using PayOSService.DTO;
using PayOSService.Services;
using SkinCareBookingSystem.Data;
using SkinCareBookingSystem.Enums;
using SkinCareBookingSystem.Implements;
using SkinCareBookingSystem.Interfaces;
using SkinCareBookingSystem.Util;

namespace SkinCareBookingSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly VNPAYPayment _vnpayPayment;
        private readonly IBookingService _bookingService;
        private readonly BookingDbContext _context;
        private readonly PayOSConfig _payOSConfig;
        private readonly IPayOSService _payOsService;
        private readonly ITherapistScheduleService _therapistScheduleService;
        // Constructor to inject the VNPAYPayment service
        public PaymentController(IBookingService bookingService, PayOSConfig payOsConfig, BookingDbContext context, IPayOSService payOsService, IOptions<PayOSConfig> payOSConfig, ITherapistScheduleService therapistScheduleService)
        {
            var tmnCode = "YourMerchantCode"; // Replace with your actual Merchant Code
            var secretKey = "YourSecretKey";  // Replace with your actual Secret Key
            _bookingService = bookingService;
            _context = context;
            _vnpayPayment = new VNPAYPayment(tmnCode, secretKey, context);
            _payOsService = payOsService;
            _payOSConfig = payOSConfig.Value;
            _therapistScheduleService = therapistScheduleService;
        }

        [HttpPost("create-payos-payment")]
        public async Task<IActionResult> CreatePayment(int bookingID)
        {
            string paymentLink = "";
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingID);
            if (booking == null) return NotFound();

            var transactions = await _context.Transactions.FirstOrDefaultAsync(x => x.BookingID == bookingID);

            if (transactions != null && !string.IsNullOrEmpty(transactions.PaymentLink))
            {
                return Ok(new { paymentLink = transactions.PaymentLink });
            }

            var code = ApplicationUtil.GetNewID();
            paymentLink = await _payOsService.CreatePaymentAsync(new CreatePaymentDTO()
            {
                OrderCode = code,
                Content = "Thanh toan",
                RequiredAmount = (int)booking.TotalPrice,
            });

            await _vnpayPayment.AddTransactionAsync(code, booking.BookingId, paymentLink, (decimal)booking.TotalPrice);

            // Do not set to Booked here; wait for payment confirmation
            return Ok(new { paymentLink });
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(int bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.BookingId == bookingId);
            if (booking == null) return NotFound();

            var timeSlot = await _context.TherapistTimeSlots.FindAsync(booking.TimeSlotId);
            if (timeSlot != null)
            {
                await _therapistScheduleService.CompletePaymentAsync(timeSlot.TimeSlotId);
            }

            return Ok();
        }

        [HttpPost("payment-return")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentReturn(
        [FromQuery] string code,
        [FromQuery] string id,
        [FromQuery] string cancel,
        [FromQuery] string status,
        [FromQuery] string orderCode)
        {
            long orderId = long.Parse(orderCode);
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.ID == orderId);
            var booking = await _context.Bookings.Include(x => x.TherapistTimeSlot).FirstOrDefaultAsync(x => x.BookingId == transaction.BookingID);

            if (code == "00" && status == "PAID")
            {
                booking.IsPaid = true;
                booking.TherapistTimeSlot.Status = SlotStatus.Booked;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, redirectUrl = _payOSConfig.ClientRedirectUrl });
            }
            else
            {
                booking.TherapistTimeSlot.Status = SlotStatus.Available;
                _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, redirectUrl = _payOSConfig.ClientRedirectUrl });
            }
        }

        // Action to redirect to VNPAY
        public IActionResult RedirectToVNPAY(string orderRef, string amount)
        {
            var paymentUrl = _vnpayPayment.BuildPaymentUrl(orderRef, amount);
            return Redirect(paymentUrl);
        }

        // Action to handle the return URL from VNPAY
        public IActionResult ReturnUrl()
        {
            var response = Request.Query;

            // Extract parameters and verify the secure hash
            string secureHash = response["vnp_SecureHash"];
            if (VerifySecureHash(response, secureHash))
            {
                string paymentStatus = response["vnp_ResponseCode"];
                if (paymentStatus == "00") // 00 means success
                {
                    return View("PaymentSuccess");  // Show success page
                }
                else
                {
                    return View("PaymentFailure");  // Show failure page
                }
            }
            return View("Error");  // In case of hash mismatch or error
        }

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
