using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;
using SkinCareBookingSystem.Data;

namespace SkinCareBookingSystem.Implements
{
    public class VNPAYPayment
    {
        private readonly string _tmnCode;
        private readonly string _secretKey;
        private readonly BookingDbContext _context;

        public VNPAYPayment(string tmnCode, string secretKey, BookingDbContext context)
        {
            _tmnCode = tmnCode;
            _secretKey = secretKey;
            _context = context;
        }

        // Build Payment URL with parameters
        public string BuildPaymentUrl(string orderRef, string amount)
        {
            string baseUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

            NameValueCollection parameters = new NameValueCollection
            {
                { "vnp_Version", "2" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _tmnCode },
                { "vnp_Amount", amount }, // Payment amount in cents
                { "vnp_OrderInfo", "Order information" },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", orderRef }, // Unique order reference
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", "https://yourwebsite.com/return-url" }, // Replace with your return URL
                { "vnp_IpAddr", "127.0.0.1" } // Optional, can use a dynamic method to get the client IP address
            };

            string secureHash = GenerateSecureHash(parameters);
            parameters.Add("vnp_SecureHash", secureHash);

            return baseUrl + "?" + ToQueryString(parameters);
        }

        // Convert NameValueCollection to query string
        public string ToQueryString(NameValueCollection nvc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string key in nvc)
            {
                sb.Append(key + "=" + HttpUtility.UrlEncode(nvc[key]) + "&");
            }
            return sb.ToString().TrimEnd('&');
        }

        // Generate the secure hash
        public string GenerateSecureHash(NameValueCollection parameters)
        {
            // Remove secure hash if present in parameters before hashing
            parameters.Remove("vnp_SecureHash");

            StringBuilder sb = new StringBuilder();
            foreach (string key in parameters)
            {
                sb.Append(key + "=" + parameters[key] + "&");
            }
            sb.Append("vnp_HashSecret=" + _secretKey); // Append secret key

            using (var sha512 = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey)))
            {
                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public async Task AddTransactionAsync(long id, int bookingID, string paymentLink, decimal amount)
        {
            var transaction = new Models.Transaction
            {
                ID = id,
                BookingID = bookingID,
                PaymentLink = paymentLink,
                Amount = amount,
                Date = DateTime.UtcNow
            };

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

        }
    }
}
