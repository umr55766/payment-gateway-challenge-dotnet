using PaymentGateway.Api.Application.Models.Requests;

namespace PaymentGateway.Api.Tests.unit.Application.Models.Requests;

public class MakePaymentRequestTests
{
    private static List<string> ValidateModel(MakePaymentRequest model)
    {
        return model.Validate();
    }

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
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("Card number must be between 14 and 19 digits"));
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidExpiryMonth()
    {
        var request = CreateValidRequest();
        request.ExpiryMonth = 13;
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("Expiry month must be between 1 and 12"));
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidExpiryYear()
    {
        var request = CreateValidRequest();
        request.ExpiryYear = DateTime.Now.Year - 1;
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("Invalid expiry year"));
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidCurrency()
    {
        var request = CreateValidRequest();
        request.Currency = "INR";
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("Currency must be one of the following"));
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidAmount()
    {
        var request = CreateValidRequest();
        request.Amount = -10;
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("Amount must be greater than 0"));
    }

    [Fact]
    public void ShouldFailValidation_ForInvalidCVV()
    {
        var request = CreateValidRequest();
        request.CVV = "12";
        var validationResults = ValidateModel(request);
        Assert.Contains(validationResults, v => v.Contains("CVV must be 3 or 4 digits"));
    }

    [Fact]
    public void ShouldPassValidation_ForValidRequest()
    {
        var request = CreateValidRequest();
        var validationResults = ValidateModel(request);
        Assert.Empty(validationResults);
    }
}