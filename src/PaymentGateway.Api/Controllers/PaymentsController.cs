using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Repositories;
using PaymentGateway.Api.Domain.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly InMemoryPaymentsRepository _inMemoryPaymentsRepository;
    private readonly PaymentService _paymentService;

    public PaymentsController(InMemoryPaymentsRepository inMemoryPaymentsRepository, PaymentService paymentService)
    {
        _inMemoryPaymentsRepository = inMemoryPaymentsRepository;
        _paymentService = paymentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MakePaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _inMemoryPaymentsRepository.GetById(id);
        return new OkObjectResult(payment);
    }

    [HttpPost]
    public async Task<ActionResult<MakePaymentResponse?>> MakePaymentAsync(MakePaymentRequest request)
    {
        return await _paymentService.MakePayment(request);
    }
}