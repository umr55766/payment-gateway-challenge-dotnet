using FluentAssertions;

using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Domain.Repositories;

namespace PaymentGateway.Api.Tests.Domain.Repositories;

public class InMemoryPaymentsRepositoryTest
{
    private readonly InMemoryPaymentsRepository _repository = new();

    [Fact]
        public void Add_ShouldAddPaymentToRepository()
        {
            Guid paymentId = Guid.NewGuid();
            var payment = new PaymentBuilder()
                .WithId(paymentId)
                .WithStatus(PaymentStatus.Authorized)
                .WithAmount(1000)
                .WithCurrency("USD")
                .WithPrecision(2)
                .WithCardNumber("1234567812345678")
                .WithCardExpiryMonth(12)
                .WithCardExpiryYear(2025)
                .WithCardCvv("123")
                .Build();
            _repository.Add(payment);
            
            var storedPayment = _repository.GetById(paymentId);

            storedPayment.Should().NotBeNull();
            storedPayment.Id.Should().Be("1");
            storedPayment.Status.Should().Be(PaymentStatus.Authorized);
        }

        [Fact]
        public void Add_ShouldThrowPaymentAlreadyExistsException_WhenPaymentAlreadyExists()
        {
            var payment1 = new PaymentBuilder()
                .WithId(Guid.NewGuid())
                .WithStatus(PaymentStatus.Authorized)
                .WithAmount(1000)
                .WithCurrency("USD")
                .WithPrecision(2)
                .WithCardNumber("1234567812345678")
                .WithCardExpiryMonth(12)
                .WithCardExpiryYear(2025)
                .WithCardCvv("123")
                .Build();
            var payment2 = new PaymentBuilder()
                .WithId(Guid.NewGuid())
                .WithStatus(PaymentStatus.Declined)
                .WithAmount(500)
                .WithCurrency("USD")
                .WithPrecision(2)
                .WithCardNumber("8765432187654321")
                .WithCardExpiryMonth(11)
                .WithCardExpiryYear(2026)
                .WithCardCvv("456")
                .Build();
            _repository.Add(payment1);

            Action act = () => _repository.Add(payment2);

            act.Should().Throw<PaymentAlreadyExistsException>()
                .WithMessage("Payment with ID 1 already exists.");
        }

        [Fact]
        public void GetById_ShouldReturnCorrectPayment()
        {
            Guid paymentId = Guid.NewGuid();
            var payment = new PaymentBuilder()
                .WithId(paymentId)
                .WithStatus(PaymentStatus.Authorized)
                .WithAmount(1000)
                .WithCurrency("USD")
                .WithPrecision(2)
                .WithCardNumber("1234567812345678")
                .WithCardExpiryMonth(12)
                .WithCardExpiryYear(2025)
                .WithCardCvv("123")
                .Build();
            _repository.Add(payment);

            var retrievedPayment = _repository.GetById(paymentId);

            retrievedPayment.Should().NotBeNull();
            retrievedPayment.Id.Should().Be("1");
        }

        [Fact]
        public void GetById_ShouldThrowPaymentNotFoundException_WhenPaymentNotFound()
        {
            Action act = () => _repository.GetById(Guid.NewGuid());

            act.Should().Throw<PaymentNotFoundException>()
                .WithMessage("Payment with ID non-existing-id not found.");
        }
}