using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Tests.unit.Domain.Entities;

public class MoneyTest
{
    [Fact]
    public void Should_Create_Money_With_Valid_Data()
    {
        var money = new Money(1000, "USD", 2);

        Assert.NotNull(money);
        Assert.Equal(1000, money.Amount);
        Assert.Equal("USD", money.Currency);
    }

    [Fact]
    public void Should_Throw_Exception_When_Amount_Is_Negative()
    {
        Assert.Throws<ArgumentException>(() => new Money(-1, "USD", 2));
    }

    [Fact]
    public void Should_Throw_Exception_When_Currency_Is_Invalid()
    {
        Assert.Throws<ArgumentException>(() => new Money(1000, "US", 2));
        Assert.Throws<ArgumentException>(() => new Money(1000, "", 2));
        Assert.Throws<ArgumentException>(() => new Money(1000, "US1", 2));
    }

    [Fact]
    public void Should_Throw_Exception_When_Precision_Is_Invalid()
    {
        Assert.Throws<ArgumentException>(() => new Money(1000, "USD", 0));
        Assert.Throws<ArgumentException>(() => new Money(1000, "USD", -1));
    }
}