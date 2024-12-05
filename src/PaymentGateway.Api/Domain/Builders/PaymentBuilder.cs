using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;

namespace PaymentGateway.Api.Domain.Builders;

public class PaymentBuilder
{
    private string _id;
    private PaymentStatus _status;
    private Money _money;
    private Card _card;
    private int _amount;
    private string _currency;
    private int _precision;
    private string _cardNumber;
    private int _cardExpiryMonth;
    private int _cardExpiryYear;
    private string _cvv;

    public PaymentBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public PaymentBuilder WithStatus(PaymentStatus status)
    {
        _status = status;
        return this;
    }

    public PaymentBuilder WithAmount(int amount)
    {
        _amount = amount;
        return this;
    }

    public PaymentBuilder WithCurrency(string currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentBuilder WithPrecision(int precision)
    {
        _precision = precision;
        return this;
    }

    public PaymentBuilder WithCardNumber(string cardNumber)
    {
        _cardNumber = cardNumber;
        return this;
    }

    public PaymentBuilder WithCardExpiryMonth(int cardExpiryMonth)
    {
        _cardExpiryMonth = cardExpiryMonth;
        return this;
    }

    public PaymentBuilder WithCardExpiryYear(int cardExpiryYear)
    {
        _cardExpiryYear = cardExpiryYear;
        return this;
    }

    public PaymentBuilder WithCardCvv(string cvv)
    {
        _cvv = cvv;
        return this;
    }
    
    public Payment Build()
    {
        if (_id == null)
            throw new ArgumentNullException(nameof(_id));
        
        _money = new Money(_amount, _currency, _precision);
        _card = new Card(_cardNumber, _cardExpiryMonth, _cardExpiryYear, _cvv);
        return new Payment(_id, _status, _money, _card);
    }
}