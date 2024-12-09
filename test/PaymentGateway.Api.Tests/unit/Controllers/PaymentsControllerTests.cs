using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Application.Models.Enums;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Infrastructure.HttpClients;
using PaymentGateway.Api.Infrastructure.Repositories;
using PaymentGateway.Api.Interfaces.Controllers;

namespace PaymentGateway.Api.Tests.unit.Controllers;

public class PaymentsControllerTests
{
    private readonly HttpClient _client;
    private readonly InMemoryPaymentsRepository _paymentsRepository;
    private readonly Mock<IBankClient> _mockBankClient;
    public PaymentsControllerTests()
    {
        _mockBankClient = new Mock<IBankClient>();
        _mockBankClient
            .Setup(client => client.MakePaymentAsync(It.IsAny<BankRequest>()))
            .ReturnsAsync(new BankResponse()
            {
                Authorized = true,
                AuthorizationCode = Guid.NewGuid().ToString()
            });

        _paymentsRepository = new InMemoryPaymentsRepository();

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        _client = webApplicationFactory.WithWebHostBuilder(builder =>
                builder.ConfigureServices(services => ((ServiceCollection)services)
                    .AddSingleton(_paymentsRepository)
                    .AddSingleton(_mockBankClient.Object)
                ))
            .CreateClient();
    }
    
    [Fact]
    public async Task ProcessAPaymentSuccessfully()
    {
        var response = await _client.PostAsync($"/api/Payments", new StringContent(JsonSerializer.Serialize(new MakePaymentRequest
        {
            Amount = 100,
            Currency = "USD",
            CardNumber = "123451234512345",
            CVV = "123",
            ExpiryMonth = 2,
            ExpiryYear = 2025,
        }), Encoding.UTF8, "application/json"));
        var paymentResponse = await response.Content.ReadFromJsonAsync<MakePaymentResponse>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }
    
    [Fact]
    public async Task ProcessAPaymentWithInvalidPayload()
    {
        var response = await _client.PostAsync($"/api/Payments", new StringContent(JsonSerializer.Serialize(new MakePaymentRequest()), Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Content.ReadAsStringAsync().Result.Contains("CVV is required").Should().BeTrue();
        response.Content.ReadAsStringAsync().Result.Contains("Currency is required").Should().BeTrue();
        response.Content.ReadAsStringAsync().Result.Contains("Card number is required").Should().BeTrue();
        response.Content.ReadAsStringAsync().Result.Contains("Invalid expiry year").Should().BeTrue();
        response.Content.ReadAsStringAsync().Result.Contains("Expiry month must be between 1 and 12").Should().BeTrue();
    }
    
    [Fact]
    public async Task ProcessAPaymentWithException()
    {
        _mockBankClient.Setup(client => client.MakePaymentAsync(It.IsAny<BankRequest>())).ThrowsAsync(new Exception());
        var response = await _client.PostAsync($"/api/Payments", new StringContent(JsonSerializer.Serialize(new MakePaymentRequest
        {
            Amount = 100,
            Currency = "USD",
            CardNumber = "123451234512345",
            CVV = "123",
            ExpiryMonth = 2,
            ExpiryYear = 2025,
        }), Encoding.UTF8, "application/json"));
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        var payment = new PaymentBuilder()
            .WithId(Guid.NewGuid())
            .WithStatus(PaymentStatus.Authorized)
            .WithAmount(100)
            .WithCurrency("USD")
            .WithPrecision(2)
            .WithCardCvv("123")
            .WithCardNumber("123451234512345")
            .WithCardExpiryMonth(2)
            .WithCardExpiryYear(2025)
            .Build();
        await _paymentsRepository.Add(payment);

        var response = await _client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        var response = await _client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}