using System.Text.Json.Serialization;

namespace Application.DTOs.Payment.PaymobRawDtos
{
    public class OrderItem
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("amount")]
        public required decimal Amount
        {
            get;
            set
            {
                field = value * 100; 
            }
        }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }
    }

    public class BillingData
    {

        [JsonPropertyName("first_name")]
        public required string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public required string LastName { get; set; }
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        [JsonPropertyName("apartment")]
        public string Apartment { get; set; } = string.Empty;

        [JsonPropertyName("floor")]
        public string Floor { get; set; } = string.Empty;

        [JsonPropertyName("street")]
        public string Street { get; set; } = string.Empty;

        [JsonPropertyName("building")]
        public string Building { get; set; } = string.Empty;

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = "+201000000000";

        [JsonPropertyName("shipping_method")]
        public string ShippingMethod { get; set; } = string.Empty;
       
        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

       
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        // Optional field often supported by Paymob:
        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = string.Empty;
    }

    public class Extras
    {
        /// <summary>
        /// Example custom extra field from your payload.
        /// You can add more properties here as needed.
        /// </summary>
        [JsonPropertyName("ee")]
        public int Ee { get; set; }


        [JsonPropertyName("confirmation_extras")]
        public object ConfirmationExtras { get; set; }
    }
}
