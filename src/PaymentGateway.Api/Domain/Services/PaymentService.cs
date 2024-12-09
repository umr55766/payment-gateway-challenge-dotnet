using System.Diagnostics;
using System.Diagnostics.Metrics;

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
    
    private readonly ActivitySource _makePaymentActivitySource;
    private readonly Counter<int> _paymentAttempts;

    public PaymentService(InMemoryPaymentsRepository paymentRepository, IBankClient bankClient, ActivitySource makePaymentActivitySource, Counter<int> paymentAttempts)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _bankClient = bankClient ?? throw new ArgumentNullException(nameof(bankClient));
        _makePaymentActivitySource = makePaymentActivitySource;
        _paymentAttempts = paymentAttempts;
    }

    public async Task<MakePaymentResponse> MakePayment(MakePaymentRequest request)
    {
        Payment payment = InitiateAPayment(request);
        BankResponse bankResponse = await ProcessPaymentWithBank(payment);
        await SaveResponseInDatabase(payment, bankResponse);
        // Publish Event/Monitoring metrics
        
        return PaymentMapper.MapToResponse(payment);
    }

    public async Task<GetPaymentResponse> GetPaymentDetails(Guid id)
    {
        return PaymentMapper.MapToGetPaymentResponse(await _paymentRepository.GetById(id));
    }

    private async Task SaveResponseInDatabase(Payment payment, BankResponse bankResponse)
    {
        payment.PatchBankResponse(bankResponse);
        await _paymentRepository.Update(payment);
    }

    private async Task<BankResponse> ProcessPaymentWithBank(Payment payment)
    {
        var bankRequest = BankRequestMapper.MapToBankRequest(payment);
        var bankResponse = await _bankClient.MakePaymentAsync(bankRequest);
        return bankResponse;
    }

    private Payment InitiateAPayment(MakePaymentRequest request)
    {
        var payment = PaymentMapper.MapToPayment(request);
        _paymentRepository.Add(payment);
        
        _paymentAttempts.Add(1);
        using var makePaymentActivity = _makePaymentActivitySource.StartActivity("MakingPayment");
        makePaymentActivity?.AddTag("currency", request.Currency);
        makePaymentActivity?.AddTag("amount", request.Amount);
        
        return payment;
    }
}