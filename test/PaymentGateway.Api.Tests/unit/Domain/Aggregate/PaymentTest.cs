using FluentAssertions;

using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Tests.unit.Domain.Aggregate;

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
        Card? card = new Card("1234567812345678", 12, 2025, "123");

        Action action = () => new Payment(paymentId, paymentStatus, null!, card);

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'money')");
    }

    [Fact]
    public void Payment_ShouldThrowException_WhenCardIsNull()
    {
        var paymentId = Guid.NewGuid();
        const PaymentStatus paymentStatus = PaymentStatus.Authorized;
        var money = new Money(1000, "USD", 2);

        Action action = () => new Payment(paymentId, paymentStatus, money, null!);

        action.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'card')");
    }

    [Fact]
    public void PatchBankResponse_ShouldUpdatePaymentWithBankResponse()
    {
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Pending,
            new Money(1000, "USD", 2),
            new Card("1234567812345678", 12, 2025, "123"));

        var bankResponse = new BankResponse { Authorized = true };
        payment.PatchBankResponse(bankResponse);
        payment.Status.Should().Be(PaymentStatus.Authorized);
        
    }

    [Fact]
    public void PatchBankResponse_ShouldUpdatePaymentWithFailureBankResponse()
    {
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Pending,
            new Money(1000, "USD", 2),
            new Card("1234567812345678", 12, 2025, "123"));

        var bankResponse = new BankResponse { Authorized = false };
        payment.PatchBankResponse(bankResponse);
        payment.Status.Should().Be(PaymentStatus.Declined);
    }
}