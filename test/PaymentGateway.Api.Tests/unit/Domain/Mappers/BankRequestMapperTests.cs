using FluentAssertions;

using PaymentGateway.Api.Application.Mappers;
using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Tests.Domain.Mappers;

public class BankRequestMapperTests
{
    [Fact]
    public void MapToBankRequest_ShouldMapPaymentToBankRequest_WhenPaymentIsValid()
    {
        var payment = new Payment(
            Guid.NewGuid(),
            PaymentStatus.Pending,
            new Money(100, "USD", 2),
            new Card("1234567890123456", 12, 2025, "123")
        );

        var result = BankRequestMapper.MapToBankRequest(payment);

        result.Should().NotBeNull();
        result.CardNumber.Should().Be("1234567890123456");
        result.ExpiryDate.Should().Be("12/2025");
        result.Currency.Should().Be("USD");
        result.Amount.Should().Be(100);
        result.Cvv.Should().Be("123");
    }

    [Fact]
    public void MapToBankRequest_ShouldThrowException_WhenPaymentIsNull()
    {
        var act = () => BankRequestMapper.MapToBankRequest(null);
        act.Should().Throw<ArgumentNullException>().WithMessage("Payment cannot be null. (Parameter 'payment')");
    }
}