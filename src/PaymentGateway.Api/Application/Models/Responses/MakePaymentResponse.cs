namespace PaymentGateway.Api.Application.Models.Responses;

public class MakePaymentResponse
{
    public required string Id { get; init; }
    public required string Status { get; init; }
    public required string? LastFourCardDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public required string? Currency { get; init; }
    public int Amount { get; init; }
}
