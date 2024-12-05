using System.Collections.Concurrent;

using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.Domain.Repositories;

public class InMemoryPaymentsRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public void Add(Payment payment)
    {
        if (!_payments.TryAdd(payment.Id, payment))
        {
            throw new PaymentAlreadyExistsException(payment.Id);
        }
    }

    public Payment GetById(Guid id)
    {
        if (!_payments.TryGetValue(id, out var payment))
        {
            throw new PaymentNotFoundException(id);
        }

        return payment;
    }
}