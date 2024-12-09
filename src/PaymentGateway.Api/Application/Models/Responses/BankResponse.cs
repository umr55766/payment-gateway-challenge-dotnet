using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Application.Models.Responses;

public class BankResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }
    
    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}