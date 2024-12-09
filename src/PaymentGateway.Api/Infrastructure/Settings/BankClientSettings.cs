namespace PaymentGateway.Api.Infrastructure.Settings
{
    public class BankClientSettings
    {
        public required string BaseUrl { get; init; }
        public int TimeoutInSeconds { get; init; }
    }
}