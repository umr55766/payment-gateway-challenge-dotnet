using FluentAssertions;

using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Tests.Domain.Builders;

public class PaymentBuilderTest
{
    [Fact]
    public void PaymentBuilder_ShouldBuildPayment_WithValidValues()
    {
        var paymentId = "12345";
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

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter '_id')");
    }
}