namespace PaymentGateway.Api.Domain.Models.Requests;

public class MakePaymentRequest
{
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string CVV { get; set; }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}