using Microsoft.Extensions.Options;

using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Infrastructure.Settings;

namespace PaymentGateway.Api.Infrastructure.HttpClients;

public class BankHttpClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankHttpClient> _logger;

    public BankHttpClient(HttpClient httpClient, ILogger<BankHttpClient> logger, IOptions<BankClientSettings> configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(configuration.Value.BaseUrl);
        _httpClient.Timeout = new TimeSpan(0, 0, configuration.Value.TimeoutInSeconds);
    }

    public async Task<BankResponse> MakePaymentAsync(BankRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(GetMakePaymentUrl(), request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BankResponse>()
                       ?? throw new HttpRequestException("Failed to deserialize bank response.");
            }

            _logger.LogError("Bank API returned an error: {StatusCode}", response.StatusCode);
            throw new HttpRequestException($"Bank API error: {response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while making payment request to the bank.");
            throw new HttpRequestException("Error communicating with the bank.", ex);
        }
    }

    private string GetMakePaymentUrl()
    {
        return "payments";
    }
}