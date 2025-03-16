using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.Implements;

namespace SkinCareBookingSystem.Controllers
{
    public class PaymentController : Controller
    {
        private readonly VNPAYPayment _vnpayPayment;

        // Constructor to inject the VNPAYPayment service
        public PaymentController()
        {
            var tmnCode = "YourMerchantCode"; // Replace with your actual Merchant Code
            var secretKey = "YourSecretKey";  // Replace with your actual Secret Key
            _vnpayPayment = new VNPAYPayment(tmnCode, secretKey);
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
