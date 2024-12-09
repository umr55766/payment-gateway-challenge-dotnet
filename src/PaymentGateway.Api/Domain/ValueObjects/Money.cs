namespace PaymentGateway.Api.Domain.ValueObjects;

public class Money
{
    public decimal Amount { get; private set; }
    public string? Currency { get; private set; }
    public int Precision { get; private set; }

    public Money(decimal amount, string? currency, int precision)
    {
        if (string.IsNullOrEmpty(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));

        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        var validCurrencies = new[] { "USD", "EUR", "GBP" };

        if (!validCurrencies.Contains(currency.ToUpper()))
            throw new ArgumentException("Invalid currency code.", nameof(currency));

        if (precision <= 0)
        {
            throw new ArgumentException("Precision must be greater than zero.", nameof(precision));
        }

        Amount = amount;
        Currency = currency.ToUpper();
        Precision = precision;
    }

    public Money()
    {
    }
}