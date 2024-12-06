using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Domain.HttpClients;

public class BankHttpClient : IBankClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BankHttpClient> _logger;
    private readonly string _baseUrl;

    public BankHttpClient(HttpClient httpClient, ILogger<BankHttpClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["BankApi:BaseUrl"];
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
        return $"{_baseUrl}/payments";
    }
}