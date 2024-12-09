﻿using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Application.Models.Requests;
using PaymentGateway.Api.Application.Models.Responses;
using PaymentGateway.Api.Application.Services;
using PaymentGateway.Api.Domain.Exceptions;

namespace PaymentGateway.Api.Interfaces.Controllers;

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
            // Push to NewRelic/Observability tool
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
            // Push to NewRelic/Observability tool
            return BadRequest(exp.Message);
        } 
    }
}