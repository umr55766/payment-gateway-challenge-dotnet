using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Application.Models.Requests;

public class BankRequest
{
    [JsonPropertyName("card_number")]
    public string? CardNumber { get; init; }
    
    [JsonPropertyName("expiry_date")]
    public required string ExpiryDate { get; init; }
    
    [JsonPropertyName("currency")]
    public required string? Currency { get; init; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; init; }
    
    [JsonPropertyName("cvv")]
    public string? Cvv { get; init; }
}