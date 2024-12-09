using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Application.Models.Requests;

public class MakePaymentRequest
{
    private const string ValidCurrencies = "^(USD|EUR|GBP)$";

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
    [RegularExpression(ValidCurrencies, ErrorMessage = "Currency must be one of the following: USD, EUR, GBP")]
    public string Currency { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public int Amount { get; set; }

    [Required(ErrorMessage = "CVV is required")]
    [StringLength(4, MinimumLength = 3, ErrorMessage = "CVV must be 3 or 4 digits")]
    [RegularExpression(@"^\d+$", ErrorMessage = "CVV must be numeric")]
    public string CVV { get; set; }
    
    public bool IsValid()
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(this, serviceProvider: null, items: null);
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results.Count == 0;
    }
}