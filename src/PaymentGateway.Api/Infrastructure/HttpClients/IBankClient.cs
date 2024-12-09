using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;

namespace PaymentGateway.Api.Infrastructure.HttpClients;

public interface IBankClient
{ 
    Task<BankResponse> MakePaymentAsync(BankRequest request);
}