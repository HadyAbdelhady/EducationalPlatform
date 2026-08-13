using Application.Features.Payment.DTOs;
using Application.Features.Payment.DTOs.PaymobRawDtos;
using Application.Features.Payment.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Features.Payment.PaymobPaymentService
{
    public class PaymentService(IOptions<PaymobSettings> paymobSettings, HttpClient httpClient) : IPaymentService
    {
        private readonly PaymobSettings _settings = paymobSettings.Value!;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<PaymentIntentionResponse> CreateIntentionAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default)
        {
            // ✅ FIX: Validate both URLs are configured
            var notificationUrl = _settings.GetNotificationUrl();
            var redirectionUrl = _settings.GetRedirectionUrl();

            if (string.IsNullOrWhiteSpace(notificationUrl))
                throw new InvalidOperationException(
                    "PayMobSettings:NotificationUrl is not configured. Paymob cannot deliver payment webhooks without it. " +
                    "Set it in appsettings.json under PaymobSettings:NotificationUrl");

            if (string.IsNullOrWhiteSpace(redirectionUrl))
                throw new InvalidOperationException(
                    "PayMobSettings:RedirectionUrl is not configured. Users won't be redirected after payment. " +
                    "Set it in appsettings.json under PaymobSettings:RedirectionUrl. " +
                    "Example: https://yourdomain.com/api/payment/Callback");

            CreatePaymentIntentionRequest intentionRequest = new()
            {
                Amount = request.Money.Amount * 100,
                Currency = request.Money.Currency,
                PaymentMethods = [Convert.ToInt32(request.PaymentMethods)],
                Items = request.Items,
                BillingData = new BillingData
                {
                    FirstName = request.Student.FirstName!,
                    LastName = request.Student.LastName!,
                    Email = request.Student.Email!,
                },
                SpecialReference = request.SpecialReference ?? Guid.NewGuid().ToString(),
                NotificationUrl = notificationUrl,
                RedirectionUrl = redirectionUrl,  // ✅ FIX: Use configured value directly
            };

            // DEBUG: Log what we're sending to Paymob
            System.Diagnostics.Debug.WriteLine("=== SENDING TO PAYMOB ===");
            System.Diagnostics.Debug.WriteLine($"Amount (cents): {intentionRequest.Amount}");
            System.Diagnostics.Debug.WriteLine($"Currency: {intentionRequest.Currency}");
            System.Diagnostics.Debug.WriteLine($"NotificationUrl: {notificationUrl}");
            System.Diagnostics.Debug.WriteLine($"RedirectionUrl: {redirectionUrl}");
            System.Diagnostics.Debug.WriteLine($"SpecialReference (for webhook correlation): {intentionRequest.SpecialReference}");
            System.Diagnostics.Debug.WriteLine($"PaymentMethods: {string.Join(", ", intentionRequest.PaymentMethods)}");
            System.Diagnostics.Debug.WriteLine($"Items: {System.Text.Json.JsonSerializer.Serialize(intentionRequest.Items)}");

            var jsonContent = JsonSerializer.Serialize(intentionRequest);
            StringContent? content = new(jsonContent, Encoding.UTF8, "application/json");

            // 2. Execute HTTP Call
            var response = await _httpClient.PostAsync("/v1/intention/", content, cancellationToken);

            // 3. Handle HTTP Errors centrally
            var error = await EnsureSuccessAsync(response, cancellationToken);
            if (error != null)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Paymob API Error: {error}");
                throw new Exception($"Error creating payment intention: {error}");
            }

            // 4. Read and Map External DTO -> Domain Model
            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            System.Diagnostics.Debug.WriteLine($"=== PAYMOB RESPONSE ===");
            System.Diagnostics.Debug.WriteLine(jsonResponse);

            var paymobResponse = JsonSerializer.Deserialize<PaymentIntentionResponse>(jsonResponse);

            if (paymobResponse == null)
                throw new Exception("Failed to deserialize Paymob response");

            System.Diagnostics.Debug.WriteLine($"✅ Payment Intention Created");
            System.Diagnostics.Debug.WriteLine($"   IntentionId: {paymobResponse.Id}");
            System.Diagnostics.Debug.WriteLine($"   ClientSecret: {paymobResponse.ClientSecret}");
            System.Diagnostics.Debug.WriteLine($"   Status: {paymobResponse.Status}");

            return paymobResponse;
        }

        public bool VerifyHmacSignature(string concatenatedHmacString, string HmacSignature)
        {
            // 1. Get HMAC secret and convert from HEX string to bytes
            var hmacSecret = GetHmac();

            if (string.IsNullOrWhiteSpace(hmacSecret))
            {
                System.Diagnostics.Debug.WriteLine("❌ HMAC Secret is not configured in PaymobSettings");
                return false;
            }

            var hmacSecretBytes = Convert.FromHexString(hmacSecret);

            // 2. Hash the concatenated string using the HMAC secret
            using var hmac = new HMACSHA512(hmacSecretBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedHmacString));
            var computedHmac = Convert.ToHexStringLower(hashBytes);

            // DEBUG: Log everything
            System.Diagnostics.Debug.WriteLine("=== HMAC VERIFICATION ===");
            System.Diagnostics.Debug.WriteLine($"HMAC Secret (hex): {hmacSecret}");
            System.Diagnostics.Debug.WriteLine($"Concatenated String: {concatenatedHmacString}");
            System.Diagnostics.Debug.WriteLine($"Computed HMAC: {computedHmac}");
            System.Diagnostics.Debug.WriteLine($"Received HMAC: {HmacSignature}");
            System.Diagnostics.Debug.WriteLine($"Match: {computedHmac.Equals(HmacSignature, StringComparison.OrdinalIgnoreCase)}");

            // 3. Compare
            if (!computedHmac.Equals(HmacSignature, StringComparison.OrdinalIgnoreCase))
            {
                System.Diagnostics.Debug.WriteLine("❌ HMAC Signature Mismatch!");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("✅ HMAC Signature Valid!");
            return true;
        }

        public string GetPublicKey() => _settings.GetPublicKey();
        public string GetHmac() => _settings.GetHmac();

        private static async Task<string> EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                return errorBody;
            }
            return null;
        }
    }
}