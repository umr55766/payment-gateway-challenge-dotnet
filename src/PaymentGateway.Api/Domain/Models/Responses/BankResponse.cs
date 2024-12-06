using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Domain.Models.Responses;

public class BankResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}