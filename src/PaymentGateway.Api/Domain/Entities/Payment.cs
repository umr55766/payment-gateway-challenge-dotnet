using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Responses;

namespace PaymentGateway.Api.Domain.Entities;

public class Payment(Guid id, PaymentStatus status, Money money, Card? card)
{
    public Guid Id { get; } = id;
    public PaymentStatus Status { get; private set; } = status;
    public Money Money { get; } = money ?? throw new ArgumentNullException(nameof(money));
    public Card Card { get; } = card ?? throw new ArgumentNullException(nameof(card));

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