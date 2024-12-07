namespace PaymentGateway.Api.Domain.Models.Requests;

public class MakePaymentRequest
{
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string CVV { get; set; }

    private static readonly List<string> ValidCurrencies = ["USD", "EUR", "GBP"];
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length < 14 || CardNumber.Length > 19 || !CardNumber.All(char.IsDigit))
        {
            throw new ArgumentException("Invalid card number");
        }

        if (ExpiryMonth is < 1 or > 12)
        {
            throw new ArgumentException("Invalid expiry month");
        }

        if (ExpiryYear < DateTime.Now.Year || (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month))
        {
            throw new ArgumentException("Invalid expiry year");
        }

        if (string.IsNullOrWhiteSpace(Currency) || !ValidCurrencies.Contains(Currency))
        {
            throw new ArgumentException("Invalid currency");
        }

        if (Amount <= 0)
        {
            throw new ArgumentException("Invalid amount");
        }

        if (string.IsNullOrWhiteSpace(CVV) || (CVV.Length < 3 || CVV.Length > 4 || !CVV.All(char.IsDigit)))
        {
            throw new ArgumentException("Invalid CVV");
        }

        return true;
    }
}