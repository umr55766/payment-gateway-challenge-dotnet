using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Application.Mappers;

public static class PaymentMapper
{
    public static Payment MapToPayment(MakePaymentRequest request)
    {
        if (request.Validate().Count != 0)
        {
            return Payment.Rejected();
        }
        
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

    public static MakePaymentResponse MapToResponse(Payment payment)
    {
        return new MakePaymentResponse
        {
            Id = payment.Id.ToString(),
            Status = payment.Status.ToString(),
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
            Status = payment.Status.ToString(),
            CardNumberLastFour = payment.Card.LastFourDigits,
            ExpiryMonth = payment.Card.ExpiryMonth,
            ExpiryYear = payment.Card.ExpiryYear,
            Currency = payment.Money.Currency,
            Amount = payment.Money.Amount
        };

        return response;
    }
}