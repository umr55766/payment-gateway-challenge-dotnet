using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Domain.Builders;
using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Domain.HttpClients;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Repositories;

namespace PaymentGateway.Api.Tests.unit.Controllers;

public class PaymentsControllerTests
{
    private readonly HttpClient _client;
    private readonly InMemoryPaymentsRepository _paymentsRepository;
    public PaymentsControllerTests()
    {
        var mockBankClient = new Mock<IBankClient>();
        mockBankClient
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
                    .AddSingleton(mockBankClient.Object)
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
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
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
        _paymentsRepository.Add(payment);

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