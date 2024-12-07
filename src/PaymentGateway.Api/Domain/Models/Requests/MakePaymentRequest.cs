using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Domain.Models.Requests;

public class MakePaymentRequest
{
    [Required(ErrorMessage = "Card number is required")]
    [StringLength(19, MinimumLength = 14, ErrorMessage = "Card number must be between 14 and 19 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Card number must be numeric")]
    public string CardNumber { get; set; }

    [Required(ErrorMessage = "Expiry month is required")]
    [Range(1, 12, ErrorMessage = "Expiry month must be between 1 and 12")]
    public int ExpiryMonth { get; set; }

    [Required(ErrorMessage = "Expiry year is required")]
    [Range(typeof(int), "2024", "3000", ErrorMessage = "Invalid expiry year")]
    public int ExpiryYear { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [RegularExpression(@"^(USD|EUR|GBP)$", ErrorMessage = "Currency must be one of the following: USD, EUR, GBP")]
    public string Currency { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "CVV is required")]
    [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "CVV must be numeric")]
    public string CVV { get; set; }

    private static readonly List<string> ValidCurrencies = ["USD", "EUR", "GBP"];
    
    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(CardNumber) || CardNumber.Length < 14 || CardNumber.Length > 19 || !CardNumber.All(char.IsDigit))
        {
            throw new ArgumentException("Invalid card number");
        }

        if (ExpiryMonth is < 1 or > 12)
        {
            throw new ArgumentException("Invalid expiry month");
        }

        if (ExpiryYear < DateTime.Now.Year || (ExpiryYear == DateTime.Now.Year && ExpiryMonth < DateTime.Now.Month))
        {
            throw new ArgumentException("Invalid expiry year");
        }

        if (string.IsNullOrWhiteSpace(Currency) || !ValidCurrencies.Contains(Currency))
        {
            throw new ArgumentException("Invalid currency");
        }

        if (Amount <= 0)
        {
            throw new ArgumentException("Invalid amount");
        }

        if (string.IsNullOrWhiteSpace(CVV) || (CVV.Length < 3 || CVV.Length > 4 || !CVV.All(char.IsDigit)))
        {
            throw new ArgumentException("Invalid CVV");
        }

        return true;
    }
}