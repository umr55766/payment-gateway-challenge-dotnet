using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Domain.Models.Responses;

public class MakePaymentResponse
{
    public string Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string LastFourCardDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}
