using FluentAssertions;

using PaymentGateway.Api.Domain.Models.Requests;

namespace PaymentGateway.Api.Tests.Domain.Models.Requests;

public class MakePaymentRequestTests
{
    [Fact]
    public void ShouldReturnFalseWhenCardNumberIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "12345",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 1000,
            CVV = "123"
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenExpiryMonthIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 15, // Invalid month
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 1000,
            CVV = "123"
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenExpiryYearIsInThePast()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2020, // Expired year
            Currency = "USD",
            Amount = 1000,
            CVV = "123"
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenCurrencyIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "INR", // Invalid currency
            Amount = 1000,
            CVV = "123"
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenAmountIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 0, // Invalid amount
            CVV = "123"
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnFalseWhenCVVIsInvalid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 1000,
            CVV = "12" // Invalid CVV
        };

        request.IsValid().Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnTrueWhenRequestIsValid()
    {
        var request = new MakePaymentRequest
        {
            CardNumber = "1234567812345678",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            Currency = "USD",
            Amount = 1000,
            CVV = "123"
        };

        request.IsValid().Should().BeTrue();
    }
}