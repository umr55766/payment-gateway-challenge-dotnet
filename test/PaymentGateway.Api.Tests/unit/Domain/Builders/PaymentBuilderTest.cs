using FluentAssertions;

using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Domain.Builders;

namespace PaymentGateway.Api.Tests.unit.Domain.Builders;

public class PaymentBuilderTest
{
    [Fact]
    public void PaymentBuilder_ShouldBuildPayment_WithValidValues()
    {
        var paymentId = Guid.NewGuid();
        const PaymentStatus paymentStatus = PaymentStatus.Authorized;
        const int amount = 1000;
        const string currency = "USD";
        const int precision = 2;
        const string cardNumber = "1234567812345678";
        const int cardExpiryMonth = 12;
        const int cardExpiryYear = 2025;
        const string cvv = "123";

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
}