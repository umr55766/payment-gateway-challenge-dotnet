using System.Diagnostics;
using System.Diagnostics.Metrics;

using FluentAssertions;

using Moq;

using PaymentGateway.Api.Application.Mappers;
using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Application.Services;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Infrastructure.HttpClients;
using PaymentGateway.Api.Infrastructure.Repositories;

namespace PaymentGateway.Api.Tests.unit.Application.Services;

public class PaymentServiceTests
{
    private readonly InMemoryPaymentsRepository _paymentRepository;
    private readonly Mock<IBankClient> _bankClientMock;
    private readonly PaymentService _paymentService;
    private readonly ActivitySource _activitySource = new("PaymentServiceTests");
    private readonly Counter<int> _counter;

    public PaymentServiceTests()
    {
        _paymentRepository = new InMemoryPaymentsRepository();
        _bankClientMock = new Mock<IBankClient>();
        var meter = new Meter("Payment", "1.0.0");
        _counter = meter.CreateCounter<int>("payment.count", description: "Counts the number of payments");
        _paymentService = new PaymentService(_paymentRepository, _bankClientMock.Object, _activitySource, _counter);
    }

    [Fact]
    public void ShouldValidateDependencies()
    {
        ((Func<PaymentService>?)(() => new PaymentService(null!, null!, _activitySource, _counter))).Should().Throw<ArgumentNullException>();
        ((Func<PaymentService>?)(() => new PaymentService(_paymentRepository, null!, _activitySource, _counter))).Should().Throw<ArgumentNullException>();
        ((Func<PaymentService>?)(() => new PaymentService(null!, _bankClientMock.Object, _activitySource, _counter))).Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task MakePaymentAsync_ValidRequest_Success()
    {
        var request = CreateAMakePaymentRequest();
        var bankResponse = new BankResponse
        {
            Authorized = true,
            AuthorizationCode = "AUTH123"
        };
        _bankClientMock.Setup(client => client.MakePaymentAsync(It.IsAny<BankRequest>()))
            .ReturnsAsync(bankResponse);

        var response = await _paymentService.MakePayment(request);

        response.Should().NotBeNull();
        response.Id.Should().Be(response.Id);
        response.Status.Should().Be(PaymentStatus.Authorized.ToString());
        var storedPayment = await _paymentRepository.GetById(Guid.Parse(response.Id));
        storedPayment.Should().NotBeNull();
        storedPayment.Status.Should().Be(PaymentStatus.Authorized);
        _bankClientMock.Verify(client => client.MakePaymentAsync(It.IsAny<BankRequest>()), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_InvalidRequest_ThrowsException()
    {
        var request = new MakePaymentRequest();
        var response = await _paymentService.MakePayment(request);
        response.Should().NotBeNull();
        response.Id.Should().NotBeNull();
        response.Status.Should().Be(PaymentStatus.Rejected.ToString());
        _bankClientMock.Verify(client => client.MakePaymentAsync(It.IsAny<BankRequest>()), Times.Never);
    }

    [Fact]
    public async Task MakePaymentAsync_BankFailure_ThrowsException()
    {
        var request = CreateAMakePaymentRequest();
        var payment = PaymentMapper.MapToPayment(request);
        _bankClientMock.Setup(client => client.MakePaymentAsync(It.IsAny<BankRequest>()))
            .ThrowsAsync(new HttpRequestException("Bank API error"));

        await Assert.ThrowsAsync<HttpRequestException>(() => _paymentService.MakePayment(request));

        await Assert.ThrowsAsync<PaymentNotFoundException>(async () => await _paymentRepository.GetById(payment.Id));
        _bankClientMock.Verify(client => client.MakePaymentAsync(It.IsAny<BankRequest>()), Times.Once);
    }

    private static MakePaymentRequest CreateAMakePaymentRequest()
    {
        return new MakePaymentRequest
        {
            Amount = 1000,
            Currency = "USD",
            CardNumber = "4111111111111111",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            CVV = "123"
        };
    }
}