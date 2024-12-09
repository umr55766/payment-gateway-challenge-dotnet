namespace PaymentGateway.Api.Domain.Entities;

public class Card
{
    public string? Number { get; set; }
    public string LastFourDigits { get; private set; }
    public int ExpiryMonth { get; private set; }
    public int ExpiryYear { get; private set; }
    public string? Cvv { get; private set; }

    public Card(string? cardNumber, int expiryMonth, int expiryYear, string? cvv)
    {
        if (string.IsNullOrEmpty(cardNumber) || !cardNumber.All(char.IsDigit) || cardNumber.Length < 14 || cardNumber.Length > 19)
            throw new ArgumentException("Card number must be between 14 and 19 digits.");
        Number = cardNumber;
        LastFourDigits = cardNumber[^4..];

        if (expiryMonth is < 1 or > 12)
            throw new ArgumentException("Expiry month must be between 1 and 12.");
        ExpiryMonth = expiryMonth;

        if (expiryYear <= DateTime.Now.Year)
            throw new ArgumentException("Expiry year must be in the future.");
        ExpiryYear = expiryYear;

        if (string.IsNullOrEmpty(cvv))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");

        if (!cvv.All(char.IsDigit))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");

        if ((cvv.Length != 3 && cvv.Length != 4))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");
        Cvv = cvv;
    }
}