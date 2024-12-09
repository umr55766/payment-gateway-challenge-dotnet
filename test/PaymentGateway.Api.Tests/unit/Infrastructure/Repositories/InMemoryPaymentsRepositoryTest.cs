using FluentAssertions;

using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Infrastructure.Repositories;

namespace PaymentGateway.Api.Tests.unit.Infrastructure.Repositories;

public class InMemoryPaymentsRepositoryTest
{
    private readonly InMemoryPaymentsRepository _repository = new();

    [Fact]
    public async Task Add_ShouldAddPaymentToRepository()
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

        await _repository.Add(payment);
        var storedPayment = await _repository.GetById(paymentId);

        storedPayment.Should().NotBeNull();
        storedPayment.Id.Should().Be(paymentId);
        storedPayment.Status.Should().Be(PaymentStatus.Authorized);
    }

    [Fact]
    public async Task Add_ShouldThrowPaymentAlreadyExistsException_WhenPaymentAlreadyExists()
    {
        Guid paymentId = Guid.NewGuid();
        var payment1 = new PaymentBuilder()
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

        var payment2 = new PaymentBuilder()
            .WithId(paymentId)
            .WithStatus(PaymentStatus.Declined)
            .WithAmount(500)
            .WithCurrency("USD")
            .WithPrecision(2)
            .WithCardNumber("8765432187654321")
            .WithCardExpiryMonth(11)
            .WithCardExpiryYear(2026)
            .WithCardCvv("456")
            .Build();
        await _repository.Add(payment1);

        Func<Task> act = async () => await _repository.Add(payment2);

        await act.Should().ThrowAsync<PaymentAlreadyExistsException>()
            .WithMessage($"Payment with ID {paymentId} already exists.");
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectPayment()
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
        await _repository.Add(payment);

        var retrievedPayment = await _repository.GetById(paymentId);

        retrievedPayment.Should().NotBeNull();
        retrievedPayment.Id.Should().Be(paymentId);
    }

    [Fact]
    public void GetById_ShouldThrowPaymentNotFoundException_WhenPaymentNotFound()
    {
        Guid paymentId = Guid.NewGuid();
        Func<Task> act = async () => await _repository.GetById(paymentId);

        act.Should().ThrowAsync<PaymentNotFoundException>()
            .WithMessage($"Payment with ID {paymentId} not found.");
    }

    [Fact]
    public void Update_ShouldThrowPaymentNotFoundException_WhenPaymentNotFound()
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
        Func<Task> act = async () => await _repository.Update(payment);

        act.Should().ThrowAsync<PaymentNotFoundException>()
            .WithMessage($"Payment with ID {paymentId} not found.");
    }

    [Fact]
    public async Task Update_ShouldUpdatePayment_WhenPaymentIsFound()
    {
        Guid paymentId = Guid.NewGuid();
        var payment = new PaymentBuilder()
            .WithId(paymentId)
            .WithStatus(PaymentStatus.Pending)
            .WithAmount(1000)
            .WithCurrency("USD")
            .WithPrecision(2)
            .WithCardNumber("1234567812345678")
            .WithCardExpiryMonth(12)
            .WithCardExpiryYear(2025)
            .WithCardCvv("123")
            .Build();
        await _repository.Add(payment);
        payment.MarkAsAuthorized();
        
        Func<Task> act = async () => await _repository.Update(payment);
        
        await act.Should().NotThrowAsync();
        payment = await _repository.GetById(paymentId);
        payment.Should().NotBeNull();
        payment.Id.Should().Be(paymentId);
        payment.Status.Should().Be(PaymentStatus.Authorized);
    }
}