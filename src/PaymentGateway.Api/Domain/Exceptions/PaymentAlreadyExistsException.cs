namespace PaymentGateway.Api.Domain.Exceptions;

public class PaymentAlreadyExistsException(Guid paymentId) : Exception($"Payment with ID {paymentId} already exists.");
