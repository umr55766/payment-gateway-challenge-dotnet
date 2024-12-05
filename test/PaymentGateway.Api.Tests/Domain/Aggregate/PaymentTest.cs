using FluentAssertions;

using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Tests.Domain.Aggregate;

public class PaymentTest
{
    [Fact]
    public void Payment_ShouldBeCreated_WithValidValues()
    {
        var paymentId = Guid.NewGuid();
        var paymentStatus = PaymentStatus.Authorized;
        var money = new Money(1000, "USD", 2);
        var card = new Card("1234567812345678", 12, 2025, "123");

        var payment = new Payment(paymentId, paymentStatus, money, card);

        payment.Should().NotBeNull();
        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be(paymentStatus);
        payment.Money.Should().Be(money);
        payment.Card.Should().Be(card);
    }

    [Fact]
    public void Payment_ShouldThrowException_WhenMoneyIsNull()
    {
        var paymentId = Guid.NewGuid();
        var paymentStatus = PaymentStatus.Authorized;
        Card card = new Card("1234567812345678", 12, 2025, "123");

        Action action = () => new Payment(paymentId, paymentStatus, null, card);

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'money')");
    }

    [Fact]
    public void Payment_ShouldThrowException_WhenCardIsNull()
    {
        var paymentId = Guid.NewGuid();
        var paymentStatus = PaymentStatus.Authorized;
        var money = new Money(1000, "USD", 2);

        Action action = () => new Payment(paymentId, paymentStatus, money, null);

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'card')");
    }
}