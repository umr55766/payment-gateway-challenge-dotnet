using System.Text.RegularExpressions;

namespace PaymentGateway.Api.Application.Models.Requests
{
    public class MakePaymentRequest
    {
        private const string ValidCurrencies = "^(USD|EUR|GBP)$";

        public string? CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string? Currency { get; set; }
        public int Amount { get; set; }
        public string? CVV { get; set; }

        public List<string> Validate()
        {
            List<string> errorMessages = [];

            // CardNumber validation
            if (string.IsNullOrEmpty(CardNumber))
                errorMessages.Add("Card number is required.");
            else if (!Regex.IsMatch(CardNumber, @"^\d+$"))
                errorMessages.Add("Card number must be numeric.");
            else if (CardNumber.Length < 14 || CardNumber.Length > 19)
                errorMessages.Add("Card number must be between 14 and 19 digits.");

            // ExpiryMonth validation
            if (ExpiryMonth < 1 || ExpiryMonth > 12)
                errorMessages.Add("Expiry month must be between 1 and 12.");

            // Expiry should in future
            if (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month)
            {
                errorMessages.Add("Expiry date must be in the future.");
            }
            else if (ExpiryYear < DateTime.Now.Year)
            {
                errorMessages.Add("Expiry date must be in the future.");
            }

            // Currency validation
            if (string.IsNullOrEmpty(Currency))
                errorMessages.Add("Currency is required.");
            else if (!Regex.IsMatch(Currency, ValidCurrencies))
                errorMessages.Add("Currency must be one of the following: USD, EUR, GBP");

            // Amount validation
            if (Amount <= 0)
                errorMessages.Add("Amount must be greater than 0.");

            // CVV validation
            if (string.IsNullOrEmpty(CVV))
                errorMessages.Add("CVV is required.");
            else if (!Regex.IsMatch(CVV, @"^\d+$"))
                errorMessages.Add("CVV must be numeric.");
            else if (CVV.Length < 3 || CVV.Length > 4)
                errorMessages.Add("CVV must be 3 or 4 digits.");

            return errorMessages;
        }
    }
}
