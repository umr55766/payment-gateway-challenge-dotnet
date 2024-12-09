using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Domain.ValueObjects;

namespace PaymentGateway.Api.Domain.Entities;

public class Payment
{
    public Guid Id { get; }
    public PaymentStatus Status { get; private set; }
    public Money Money { get; }
    public Card Card { get; }

    private Payment()
    {
        Id = Guid.NewGuid();
        Status = PaymentStatus.Pending;
        Card = new Card();
        Money = new Money();
    }
    
    public Payment(Guid id, PaymentStatus status, Money money, Card? card)
    {
        Id = id;
        Status = status;
        Money = money ?? throw new ArgumentNullException(nameof(money));
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }
    public void MarkAsAuthorized()
    {
        this.Status = PaymentStatus.Authorized;
    }

    public void PatchBankResponse(BankResponse bankResponse)
    {
        ArgumentNullException.ThrowIfNull(bankResponse);
        this.Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }

    public static Payment Rejected()
    {
        var payment = new Payment { Status = PaymentStatus.Rejected };
        return payment;
    }

    public bool IsRejected()
    {
        return this.Status == PaymentStatus.Rejected;
    }
}