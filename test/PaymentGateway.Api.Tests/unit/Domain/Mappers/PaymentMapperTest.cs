using PaymentGateway.Api.Application.Mappers;
using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Tests.Domain.Mappers;

public class PaymentMapperTest
{
    [Fact]
    public void MapToPayment_ValidRequest_ReturnsPayment()
    {
        // Arrange
        var request = new MakePaymentRequest
        {
            Amount = 1000,
            Currency = "USD",
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            CVV = "123"
        };

        // Act
        var payment = PaymentMapper.MapToPayment(request);

        // Assert
        Assert.NotNull(payment);
        Assert.Equal(request.Amount, payment.Money.Amount);
        Assert.Equal(request.Currency, payment.Money.Currency);
        Assert.Equal(request.CardNumber, payment.Card.Number);
        Assert.Equal(request.ExpiryMonth, payment.Card.ExpiryMonth);
        Assert.Equal(request.ExpiryYear, payment.Card.ExpiryYear);
        Assert.Equal(request.CVV, payment.Card.Cvv);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void MapToBankRequest_ValidPayment_ReturnsBankRequest()
    {
        // Arrange
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Pending,
            new Money(1000, "USD", 2),
            new Card("4111111111111111", 12, 2030, "123")
        );

        // Act
        var bankRequest = PaymentMapper.MapToBankRequest(payment);

        // Assert
        Assert.NotNull(bankRequest);
        Assert.Equal(payment.Money.Amount, bankRequest.Amount);
        Assert.Equal(payment.Money.Currency, bankRequest.Currency);
        Assert.Equal(payment.Card.Number, bankRequest.CardNumber);
        Assert.Equal($"{payment.Card.ExpiryMonth}/{payment.Card.ExpiryYear}", bankRequest.ExpiryDate);
        Assert.Equal(payment.Card.Cvv, bankRequest.Cvv);
    }

    [Fact]
    public void MapToMakePaymentResponse_ValidPayment_ReturnsResponse()
    {
        // Arrange
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Authorized,
            new Money(1000, "USD", 2),
            new Card("4111111111111111", 12, 2030, "123")
        );

        // Act
        var response = PaymentMapper.MapToResponse(payment);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(payment.Id.ToString(), response.Id);
        Assert.Equal(payment.Money.Amount, response.Amount);
        Assert.Equal(payment.Money.Currency, response.Currency);
        Assert.Equal(payment.Status.ToString(), response.Status);
    }

    [Fact]
    public void MapToMakePaymentResponse_PaymentWithoutAuthorizationCode_ReturnsResponse()
    {
        // Arrange
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Declined,
            new Money(1000, "USD", 2),
            new Card("4111111111111111", 12, 2030, "123")
        );

        // Act
        var response = PaymentMapper.MapToResponse(payment);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(payment.Id.ToString(), response.Id);
        Assert.Equal(payment.Money.Amount, response.Amount);
        Assert.Equal(payment.Money.Currency, response.Currency);
        Assert.Equal(payment.Status.ToString(), response.Status);
    }
}