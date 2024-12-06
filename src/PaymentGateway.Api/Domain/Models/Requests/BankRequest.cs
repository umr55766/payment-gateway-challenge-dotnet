namespace PaymentGateway.Api.Domain.Models.Requests;

public class BankRequest
{
    public string CardNumber { get; set; }
    public string ExpiryDate { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string Cvv { get; set; }
}