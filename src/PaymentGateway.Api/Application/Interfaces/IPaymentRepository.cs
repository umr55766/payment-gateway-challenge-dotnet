using PaymentGateway.Api.Domain.Aggregate;

namespace PaymentGateway.Api.Application.Interfaces;

public interface IPaymentRepository
{
    Task Add(Payment payment);
    Task<Payment> GetById(Guid id);
    Task Update(Payment payment);
}