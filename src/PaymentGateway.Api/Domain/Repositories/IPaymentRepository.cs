using PaymentGateway.Api.Domain.Aggregate;

namespace PaymentGateway.Api.Domain.Repositories;

public interface IPaymentRepository
{
    Task Add(Payment payment);
    Task<Payment> GetById(Guid id);
    Task Update(Payment payment);
}