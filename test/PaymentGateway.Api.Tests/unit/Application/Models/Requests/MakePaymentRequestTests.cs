using FluentAssertions;

using PaymentGateway.Api.Application.Models.Requests;

namespace PaymentGateway.Api.Tests.unit.Application.Models.Requests;

public class MakePaymentRequestTests
{
    private static MakePaymentRequest CreateValidRequest()
    {
        return new MakePaymentRequest
        {
            CardNumber = "1234567890123456",
            ExpiryMonth = 12,
            ExpiryYear = DateTime.Now.Year + 1,
            Currency = "USD",
            Amount = 100,
            CVV = "123"
        };
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidCardNumber()
    {
        var request = CreateValidRequest();
        request.CardNumber = "123";
        request.Validate().Should().Contain("Card number must be between 14 and 19 digits.");
        request.CardNumber = "abc";
        request.Validate().Should().Contain("Card number must be numeric.");
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidExpiryMonth()
    {
        var request = CreateValidRequest();
        request.ExpiryMonth = 13;
        request.Validate().Should().Contain("Expiry month must be between 1 and 12.");
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidExpiryDate()
    {
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.Now.Year - 1;
        request.Validate().Should().Contain("Expiry date must be in the future.");
        
        request.ExpiryYear = DateTime.Now.Year;
        request.ExpiryMonth = DateTime.Now.Month - 1;
        request.Validate().Should().Contain("Expiry date must be in the future.");
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidCurrency()
    {
        var request = CreateValidRequest();
        request.Currency = "INR";
        request.Validate().Should().Contain("Currency must be one of the following: USD, EUR, GBP");
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidAmount()
    {
        var request = CreateValidRequest();
        request.Amount = -10;
        request.Validate().Should().Contain("Amount must be greater than 0.");
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidCVV()
    {
        var request = CreateValidRequest();
        request.CVV = "12";
        request.Validate().Should().Contain("CVV must be 3 or 4 digits.");
        request.CVV = "abc";
        request.Validate().Should().Contain("CVV must be numeric.");
    }

    [Fact]
    public void ShouldPassValidation_ForValidRequest()
    {
        CreateValidRequest().Validate().Should().BeEmpty();
    }
}