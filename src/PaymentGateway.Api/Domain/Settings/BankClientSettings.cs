namespace PaymentGateway.Api.Domain.Settings
{
    public class BankClientSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutInSeconds { get; set; }
    }
}