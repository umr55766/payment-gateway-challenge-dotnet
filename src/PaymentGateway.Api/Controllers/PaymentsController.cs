using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Domain.Exceptions;
using PaymentGateway.Api.Domain.Models.Requests;
using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        try
        {
            return await _paymentService.GetPaymentDetails(id);
        }
        catch (PaymentNotFoundException exp)
        {
            return NotFound(exp.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MakePaymentResponse?>> MakePaymentAsync(MakePaymentRequest request)
    {
        try
        {
            return await _paymentService.MakePayment(request);
        }
        catch (Exception exp)
        {
            return BadRequest(exp.Message);
        } 
    }
}