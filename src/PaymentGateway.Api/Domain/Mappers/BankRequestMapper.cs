using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Models.Requests;

namespace PaymentGateway.Api.Domain.Mappers;

public class BankRequestMapper
{
    public static BankRequest MapToBankRequest(Payment payment)
    {
        if (payment == null)
        {
            throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");
        }

        return new BankRequest
        {
            CardNumber = payment.Card.Number,
            ExpiryDate = $"{payment.Card.ExpiryMonth:D2}/{payment.Card.ExpiryYear}",
            Currency = payment.Money.Currency,
            Amount = payment.Money.Amount,
            Cvv = payment.Card.Cvv
        };
    }
}