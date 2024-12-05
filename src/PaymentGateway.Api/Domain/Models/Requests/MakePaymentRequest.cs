namespace PaymentGateway.Api.Domain.Models.Requests;

public class MakePaymentRequest
{
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string CVV { get; set; }

    private static readonly List<string> ValidCurrencies = new List<string> { "USD", "EUR", "GBP" };
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length < 14 || CardNumber.Length > 19 || !CardNumber.All(char.IsDigit))
        {
            return false;
        }

        if (ExpiryMonth < 1 || ExpiryMonth > 12)
        {
            return false;
        }

        if (ExpiryYear < DateTime.Now.Year || (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(Currency) || !ValidCurrencies.Contains(Currency))
        {
            return false;
        }

        if (Amount <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(CVV) || (CVV.Length < 3 || CVV.Length > 4 || !CVV.All(char.IsDigit)))
        {
            return false;
        }

        return true;
    }
}