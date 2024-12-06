using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;
using Moq.Protected;

using PaymentGateway.Api.Domain.HttpClients;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Tests.Domain.Adapters;

public class BankHttpClientTest
{
    private readonly Mock<HttpMessageHandler> _mockedHandler;
    private readonly BankHttpClient _bankHttpClient;
    
    public BankHttpClientTest()
    {
        Mock<ILogger<BankHttpClient>> loggerMock = new();
        _mockedHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockedHandler.Object);
        
        var configData = new Dictionary<string, string>
        {
            { "BankApi:BaseUrl", "http://localhost:8080" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();
        
        _bankHttpClient = new BankHttpClient(httpClient, loggerMock.Object, configuration);
    }

    [Fact]
    public async Task MakePaymentAsync_ShouldReturnBankResponse_WhenRequestIsValid()
    {
        var expectedResponse = new BankResponse
        {
            Authorized = true,
            AuthorizationCode = "abc123"
        };
        MockedHandlerFor(expectedResponse, HttpStatusCode.OK);
        
        var request = new BankRequest
        {
            CardNumber = "2222405343248877",
            ExpiryDate = "04/2025",
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };
        var result = await _bankHttpClient.MakePaymentAsync(request);

        result.Should().NotBeNull();
        result.Authorized.Should().BeTrue();
        result.AuthorizationCode.Should().Be("abc123");
        AssertBankIsCalled();
    }

    [Fact]
    public async Task MakePaymentAsync_ShouldThrowException_WhenResponseIsNotSuccessful()
    {
        MockedHandlerFor(new BankResponse(), HttpStatusCode.BadRequest);
        BankRequest request = new()
        {
            CardNumber = "2222405343248877",
            ExpiryDate = "04/2025",
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };
        var act = async () => await _bankHttpClient.MakePaymentAsync(request);

        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Bank API error: BadRequest");
        AssertBankIsCalled();
    }

    [Fact]
    public async Task MakePaymentAsync_ShouldThrowException_WhenHttpClientThrowsException()
    {
        _mockedHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));
        
        var request = new BankRequest
        {
            CardNumber = "2222405343248877",
            ExpiryDate = "04/2025",
            Currency = "GBP",
            Amount = 100,
            Cvv = "123"
        };
        var act = async () => await _bankHttpClient.MakePaymentAsync(request);

        await act.Should().ThrowAsync<HttpRequestException>().WithMessage("Network error");
        AssertBankIsCalled();
    }

    private void MockedHandlerFor(BankResponse expectedResponse, HttpStatusCode httpStatusCode)
    {
        _mockedHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = httpStatusCode,
                Content = JsonContent.Create(expectedResponse)
            });
    }

    private void AssertBankIsCalled()
    {
        _mockedHandler.Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri == new Uri("http://localhost:8080" + "/payments")),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}