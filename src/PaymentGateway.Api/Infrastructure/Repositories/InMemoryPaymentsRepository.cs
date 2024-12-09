using System.Collections.Concurrent;

using PaymentGateway.Api.Application.Interfaces;
using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.Infrastructure.Repositories;

public class InMemoryPaymentsRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public Task Add(Payment payment)
    {
        if (!_payments.TryAdd(payment.Id, payment))
        {
            throw new PaymentAlreadyExistsException(payment.Id);
        }
        
        return Task.CompletedTask;
    }

    public Task<Payment> GetById(Guid id)
    {
        if (!_payments.TryGetValue(id, out var payment))
        {
            throw new PaymentNotFoundException(id);
        }

        return Task.FromResult(payment);
    }
    
    public Task Update(Payment payment)
    {
        if (!_payments.TryGetValue(payment.Id, out var _))
        {
            throw new PaymentNotFoundException(payment.Id);
        }
        
        _payments[payment.Id] = payment;
        return Task.CompletedTask;
    }
}