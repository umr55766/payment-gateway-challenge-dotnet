using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Domain.Mappers;

public class PaymentMapper
{
    public static Payment MapToPayment(MakePaymentRequest request)
    {
        return new PaymentBuilder()
            .WithId(Guid.NewGuid())
            .WithAmount(request.Amount)
            .WithCurrency(request.Currency)
            .WithPrecision(2) // Assuming default precision
            .WithCardNumber(request.CardNumber)
            .WithCardExpiryMonth(request.ExpiryMonth)
            .WithCardExpiryYear(request.ExpiryYear)
            .WithCardCvv(request.CVV)
            .WithStatus(PaymentStatus.Pending)
            .Build();
    }

    public static BankRequest MapToBankRequest(Payment payment)
    {
        return new BankRequest
        {
            CardNumber = payment.Card.Number,
            ExpiryDate = $"{payment.Card.ExpiryMonth:D2}/{payment.Card.ExpiryYear}",
            Currency = payment.Money.Currency,
            Amount = payment.Money.Amount,
            Cvv = payment.Card.Cvv
        };
    }

    public static MakePaymentResponse MapToResponse(Payment payment)
    {
        return new MakePaymentResponse
        {
            Id = payment.Id.ToString(),
            Status = payment.Status,
            LastFourCardDigits = payment.Card.LastFourDigits,
            ExpiryMonth = payment.Card.ExpiryMonth,
            ExpiryYear = payment.Card.ExpiryYear,
            Currency = payment.Money.Currency,
            Amount = payment.Money.Amount
        };
    }

    public static GetPaymentResponse MapToGetPaymentResponse(Payment payment)
    {
        var response = new GetPaymentResponse
        {
            Id = payment.Id,
            Status = payment.Status,
            CardNumberLastFour = payment.Card.LastFourDigits,
            ExpiryMonth = payment.Card.ExpiryMonth,
            ExpiryYear = payment.Card.ExpiryYear,
            Currency = payment.Money.Currency,
            Amount = payment.Money.Amount
        };

        return response;
    }
}