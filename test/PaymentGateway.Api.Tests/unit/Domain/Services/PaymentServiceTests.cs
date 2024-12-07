using FluentAssertions;

using Moq;

using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Domain.HttpClients;
using PaymentGateway.Api.Domain.Mappers;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Repositories;
using PaymentGateway.Api.Domain.Services;

namespace PaymentGateway.Api.Tests.Domain.Services;

public class PaymentServiceTests
{
    private readonly InMemoryPaymentsRepository _paymentRepository;
    private readonly Mock<IBankClient> _bankClientMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentRepository = new InMemoryPaymentsRepository();
        _bankClientMock = new Mock<IBankClient>();
        _paymentService = new PaymentService(_paymentRepository, _bankClientMock.Object);
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
        var storedPayment = _paymentRepository.GetById(Guid.Parse(response.Id));
        storedPayment.Should().NotBeNull();
        storedPayment.Status.Should().Be(PaymentStatus.Authorized);
        _bankClientMock.Verify(client => client.MakePaymentAsync(It.IsAny<BankRequest>()), Times.Once);
    }

    [Fact]
    public async Task MakePaymentAsync_InvalidRequest_ThrowsException()
    {
        var request = new MakePaymentRequest();
        await Assert.ThrowsAsync<ArgumentException>(() => _paymentService.MakePayment(request));
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

        Assert.Throws<PaymentNotFoundException>(() => _paymentRepository.GetById(payment.Id));
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