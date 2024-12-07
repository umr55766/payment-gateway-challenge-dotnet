using FluentAssertions;

using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.Models.Requests;

namespace PaymentGateway.Api.Tests.Domain.Builders;

public class PaymentBuilderTest
{
    [Fact]
    public void PaymentBuilder_ShouldBuildPayment_WithValidValues()
    {
        var paymentId = Guid.NewGuid();
        var paymentStatus = PaymentStatus.Authorized;
        var amount = 1000;
        var currency = "USD";
        var precision = 2;
        var cardNumber = "1234567812345678";
        var cardExpiryMonth = 12;
        var cardExpiryYear = 2025;
        var cvv = "123";

        var payment = new PaymentBuilder()
            .WithId(paymentId)
            .WithStatus(paymentStatus)
            .WithAmount(amount)
            .WithCurrency(currency)
            .WithPrecision(precision)
            .WithCardNumber(cardNumber)
            .WithCardExpiryMonth(cardExpiryMonth)
            .WithCardExpiryYear(cardExpiryYear)
            .WithCardCvv(cvv)
            .Build();

        payment.Should().NotBeNull();
        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be(paymentStatus);
        payment.Money.Amount.Should().Be(amount);
        payment.Money.Currency.Should().Be(currency);
        payment.Money.Precision.Should().Be(precision);
        payment.Card.Number.Should().Be(cardNumber);
        payment.Card.ExpiryMonth.Should().Be(cardExpiryMonth);
        payment.Card.ExpiryYear.Should().Be(cardExpiryYear);
        payment.Card.Cvv.Should().Be(cvv);
    }

    [Fact]
    public void PaymentBuilder_ShouldThrowException_WhenRequiredFieldsAreMissing()
    {
        Action action = () => new PaymentBuilder().Build();

        action.Should().Throw<ArgumentException>()
            .WithMessage("Value cannot be null. (Parameter '_id')");
    }
    
    [Fact]
    public void FromRequest_ShouldPopulateBuilder_WhenRequestIsValid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "USD",
            Amount = 1000,
            CVV = "123"
        };
        var builder = new PaymentBuilder();

        builder.FromRequest(request);

        var payment = builder.Build();
        payment.Should().NotBeNull();
        payment.Money.Amount.Should().Be(1000);
        payment.Money.Currency.Should().Be("USD");
        payment.Card.Number.Should().Be("1234567890123456");
        payment.Card.ExpiryMonth.Should().Be(12);
        payment.Card.ExpiryYear.Should().Be(DateTime.Now.Year + 1);
        payment.Card.Cvv.Should().Be("123");
        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void FromRequest_ShouldThrowException_WhenRequestIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "", // Invalid card number
            ExpiryMonth = 13, // Invalid month
            ExpiryYear = DateTime.Now.Year - 1, // Invalid year
            Currency = "XYZ", // Invalid currency
            Amount = -100, // Invalid amount
            CVV = "12" // Invalid CVV
        };
        var builder = new PaymentBuilder();

        var act = () => builder.FromRequest(request);

        act.Should().Throw<InvalidOperationException>().WithMessage("Invalid MakePaymentRequest.");
    }
}