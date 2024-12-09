using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Application.Models.Requests;

public class BankRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; set; }
    
    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; set; }
    
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
    
    [JsonPropertyName("cvv")]
    public string Cvv { get; set; }
}