using FluentAssertions;

using PaymentGateway.Api.Domain.ValueObjects;

namespace PaymentGateway.Api.Tests.unit.Domain.ValueObjects;

public class CardTest
{
    [Fact]
    public void Should_Create_Card_With_Valid_Data()
    {
        var card = new Card("1234567812345678", 12, 2025, "123");

        card.Should().NotBeNull();
        card.LastFourDigits.Should().Be("5678");
        card.ExpiryMonth.Should().Be(12);
        card.ExpiryYear.Should().Be(2025);
        card.Cvv.Should().Be("123");
    }

    [Theory]
    [InlineData("12345678", 12, 2025, "123", "Card number must be between 14 and 19 digits.")]
    [InlineData("1234567812345678", 0, 2025, "123", "Expiry month must be between 1 and 12.")]
    [InlineData("1234567812345678", 13, 2025, "123", "Expiry month must be between 1 and 12.")]
    [InlineData("1234567812345678", 12, 2022, "123", "Expiry year must be in the future.")]
    [InlineData("1234567812345678", 12, 2025, "12", "CVV must be a numeric string with 3 or 4 digits.")]
    [InlineData("1234567812345678", 12, 2025, "12345", "CVV must be a numeric string with 3 or 4 digits.")]
    [InlineData("1234567812345678", 12, 2025, "", "CVV must be a numeric string with 3 or 4 digits.")]
    [InlineData("1234567812345678", 12, 2025, null, "CVV must be a numeric string with 3 or 4 digits.")]
    [InlineData("1234567812345678", 12, 2025, "abc", "CVV must be a numeric string with 3 or 4 digits.")]
    public void Should_Throw_Exception_When_Creating_Card_With_Invalid_Data(
        string? cardNumber, int expiryMonth, int expiryYear, string? cvv, string expectedErrorMessage)
    {
        var exception = Assert.Throws<ArgumentException>(() => new Card(cardNumber, expiryMonth, expiryYear, cvv));
        exception.Message.Should().Be(expectedErrorMessage);
    }
}