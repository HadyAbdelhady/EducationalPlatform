using Application.DTOs.Payment;
using Application.DTOs.Payment.PaymobRawDtos;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services.PaymobPaymentService
{
    public class PaymentService(IOptions<PaymobSettings> paymobSettings, HttpClient httpClient) : IPaymentService
    {
        private readonly PaymobSettings _settings = paymobSettings.Value!;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<PaymentIntentionResponse> CreateIntentionAsync(PaymentInitiationRequest request, CancellationToken cancellationToken = default)
        {
            CreatePaymentIntentionRequest intentionRequest = new()
            {
                Amount = request.Money.Amount,
                Currency = request.Money.Currency,
                PaymentMethods = [Convert.ToInt32(request.PaymentMethods)],
                Items = request.Items,
                BillingData = new BillingData
                {
                    FirstName = request.Student.FirstName!,
                    LastName = request.Student.LastName!,
                    Email = request.Student.Email!,
                },
            };
            var jsonContent = JsonSerializer.Serialize(intentionRequest);
            StringContent? content = new(jsonContent, Encoding.UTF8, "application/json");

            // 2. Execute HTTP Call
            var response = await _httpClient.PostAsync("/v1/intention/", content, cancellationToken);

            // 3. Handle HTTP Errors centrally
            var error = await EnsureSuccessAsync(response, cancellationToken);
            if (error != null)
            {
                throw new Exception($"Error creating payment intention: {error}");
            }
            // 4. Read and Map External DTO -> Domain Model
            var jsonResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            var paymobResponse = JsonSerializer.Deserialize<PaymentIntentionResponse>(jsonResponse);
            return paymobResponse;
        }

        public bool VerifyHmacSignature(string concatenatedHmacString, string HmacSignature)
        {
            // 1. Hash the concatenated string using your HMAC Secret (from Paymob settings)
            var hmacSecret = _settings.GetHmac();
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(hmacSecret));
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(concatenatedHmacString));
            var computedHmac = Convert.ToHexStringLower(hashBytes);

            // 2. Compare with the signature sent by Paymob in the URL query
            if (!computedHmac.Equals(HmacSignature, StringComparison.CurrentCultureIgnoreCase))
                return false; // SECURITY ALERT: The payload was tampered with or didn't come from Paymob!

            return true;
        }
        public string GetPublicKey() => _settings.GetPublicKey();
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

