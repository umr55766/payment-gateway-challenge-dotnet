using System.Diagnostics;
using System.Diagnostics.Metrics;

using PaymentGateway.Api.Application.Mappers;
using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Domain.Entities;
using PaymentGateway.Api.Infrastructure.HttpClients;
using PaymentGateway.Api.Infrastructure.Repositories;

namespace PaymentGateway.Api.Application.Services;

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

        if (payment.IsRejected())
        {
            return PaymentMapper.MapToResponse(payment);
        }

        BankResponse bankResponse = await ProcessPaymentWithBank(payment);
        await SaveResponseInDatabase(payment, bankResponse);
        // Publish Event/Monitoring metrics
        
        return PaymentMapper.MapToResponse(payment);
    }

    public async Task<GetPaymentResponse> GetPaymentDetails(Guid id)
    {
        Payment payment = await _paymentRepository.GetById(id);
        return PaymentMapper.MapToGetPaymentResponse(payment);
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
        using var makePaymentActivity = _makePaymentActivitySource.StartActivity("MakingPayment");

        Payment payment = PaymentMapper.MapToPayment(request);   
        _paymentRepository.Add(payment);
        
        _paymentAttempts.Add(1);
        makePaymentActivity?.AddTag("currency", request.Currency);
        makePaymentActivity?.AddTag("amount", request.Amount);
        
        return payment;
    }
}