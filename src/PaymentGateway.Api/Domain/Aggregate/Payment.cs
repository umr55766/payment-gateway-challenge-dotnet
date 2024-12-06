using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Domain.Aggregate;

public class Payment
{
    public Guid Id { get; }
    public PaymentStatus Status { get; set; }
    public Money Money { get; }
    public Card Card { get; }

    public Payment(Guid id, PaymentStatus status, Money money, Card card)
    {
        Id = id;
        Status = status;
        Money = money ?? throw new ArgumentNullException(nameof(money));
        Card = card ?? throw new ArgumentNullException(nameof(card));
    }

    public void MarkAsAuthorized()
    {
        Status = PaymentStatus.Authorized;
    }

    public void PatchBankResponse(BankResponse bankResponse)
    {
        ArgumentNullException.ThrowIfNull(bankResponse);
        Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
    }
}