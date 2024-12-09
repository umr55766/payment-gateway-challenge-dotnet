using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Domain.Entities;

namespace PaymentGateway.Api.Domain.Builders;

public class PaymentBuilder
{
    private Guid _id;
    private PaymentStatus _status;
    private Money? _money;
    private Card? _card;
    private int _amount;
    private string? _currency;
    private int _precision;
    private string? _cardNumber;
    private int _cardExpiryMonth;
    private int _cardExpiryYear;
    private string? _cvv;

    public PaymentBuilder WithId(Guid id)
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

    public PaymentBuilder WithCurrency(string? currency)
    {
        _currency = currency;
        return this;
    }

    public PaymentBuilder WithPrecision(int precision)
    {
        _precision = precision;
        return this;
    }

    public PaymentBuilder WithCardNumber(string? cardNumber)
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

    public PaymentBuilder WithCardCvv(string? cvv)
    {
        _cvv = cvv;
        return this;
    }
    
    public Payment Build()
    {
        if (_id == Guid.Empty)
            throw new ArgumentNullException(nameof(_id));
        
        _money = new Money(_amount, _currency, _precision);
        _card = new Card(_cardNumber, _cardExpiryMonth, _cardExpiryYear, _cvv);
        return new Payment(_id, _status, _money, _card);
    }

    public PaymentBuilder FromRequest(MakePaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!request.IsValid())
        {
            throw new InvalidOperationException("Invalid MakePaymentRequest.");
        }

        _id = Guid.NewGuid();
        _status = PaymentStatus.Pending;
        _amount = request.Amount;
        _currency = request.Currency;
        _precision = 2;
        _cardNumber = request.CardNumber;
        _cardExpiryMonth = request.ExpiryMonth;
        _cardExpiryYear = request.ExpiryYear;
        _cvv = request.CVV;

        return this;
    }
}