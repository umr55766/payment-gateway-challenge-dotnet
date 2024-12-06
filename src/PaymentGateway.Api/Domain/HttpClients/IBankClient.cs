using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Domain.HttpClients;

public interface IBankClient
{ 
    Task<BankResponse> MakePaymentAsync(BankRequest request);
}