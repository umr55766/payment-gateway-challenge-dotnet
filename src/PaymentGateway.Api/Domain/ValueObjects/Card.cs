using System.Security.Cryptography;
using System.Text;

namespace PaymentGateway.Api.Domain.ValueObjects;

public class Card
{
    public string? LastFourDigits { get; private set; }
    public int ExpiryMonth { get; private set; }
    public int ExpiryYear { get; private set; }
    public string? Cvv { get; private set; }
    private string? _encryptedNumber;
    public string? Number
    {
        get
        {
            return string.IsNullOrEmpty(_encryptedNumber) ? null : Decrypt(_encryptedNumber);
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            LastFourDigits = value[^4..];
            _encryptedNumber = Encrypt(value);
        }
    }

    public Card()
    {
    }
    
    public Card(string? cardNumber, int expiryMonth, int expiryYear, string? cvv)
    {
        if (string.IsNullOrEmpty(cardNumber) || !cardNumber.All(char.IsDigit) || cardNumber.Length < 14 || cardNumber.Length > 19)
            throw new ArgumentException("Card number must be between 14 and 19 digits.");
        Number = cardNumber;
        LastFourDigits = cardNumber[^4..];

        if (expiryMonth is < 1 or > 12)
            throw new ArgumentException("Expiry month must be between 1 and 12.");
        ExpiryMonth = expiryMonth;

        if (expiryYear < DateTime.Now.Year)
            throw new ArgumentException("Expiry year must be in the future.");
        if (expiryMonth < DateTime.Now.Month)
            throw new ArgumentException("Expiry year must be in the future.");
        ExpiryYear = expiryYear;

        if (string.IsNullOrEmpty(cvv))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");

        if (!cvv.All(char.IsDigit))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");

        if ((cvv.Length != 3 && cvv.Length != 4))
            throw new ArgumentException("CVV must be a numeric string with 3 or 4 digits.");
        Cvv = cvv;
    }
    
    private static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String("7Wr3jvhT+dlzXM6Wp93F4rF6wUbWTQh20jxYXZb4Q9c=");
        aes.IV = Convert.FromBase64String("Z1dx3EjxHjD2xn+WT9fKFA==");

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        return Convert.ToBase64String(encryptedBytes);
    }

    private static string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = Convert.FromBase64String("7Wr3jvhT+dlzXM6Wp93F4rF6wUbWTQh20jxYXZb4Q9c=");
        aes.IV = Convert.FromBase64String("Z1dx3EjxHjD2xn+WT9fKFA==");

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}