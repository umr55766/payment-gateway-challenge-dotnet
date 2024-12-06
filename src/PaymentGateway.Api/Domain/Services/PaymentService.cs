using PaymentGateway.Api.Domain.Aggregate;
using PaymentGateway.Api.Domain.HttpClients;
using PaymentGateway.Api.Domain.Mappers;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Repositories;

namespace PaymentGateway.Api.Domain.Services;

public class PaymentService
{
    private readonly InMemoryPaymentsRepository _paymentRepository;
    private readonly IBankClient _bankClient;

    public PaymentService(InMemoryPaymentsRepository paymentRepository, IBankClient bankClient)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _bankClient = bankClient ?? throw new ArgumentNullException(nameof(bankClient));
    }

    public async Task<MakePaymentResponse> MakePayment(MakePaymentRequest request)
    {
        Payment payment = InitiateAPayment(request);
        BankResponse bankResponse = await ProcessPaymentWithBank(payment);
        SaveResponseInDatabase(payment, bankResponse);

        return PaymentMapper.MapToResponse(payment);
    }

    private void SaveResponseInDatabase(Payment payment, BankResponse bankResponse)
    {
        payment.PatchBankResponse(bankResponse);
        _paymentRepository.Update(payment);
    }

    private async Task<BankResponse> ProcessPaymentWithBank(Payment payment)
    {
        var bankRequest = BankRequestMapper.MapToBankRequest(payment);
        var bankResponse = await _bankClient.MakePaymentAsync(bankRequest);
        return bankResponse;
    }

    private Payment InitiateAPayment(MakePaymentRequest request)
    {
        if (!request.IsValid())
            throw new ArgumentException("Invalid payment request");

        var payment = PaymentMapper.MapToPayment(request);
        _paymentRepository.Add(payment);
        return payment;
    }

    public Payment ProcessPayment(Payment payment)
    {
        throw new NotImplementedException();
    }
}