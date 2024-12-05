namespace PaymentGateway.Api.Domain.Exceptions;

public class PaymentNotFoundException(Guid paymentId) : Exception($"Payment with ID {paymentId} not found.");