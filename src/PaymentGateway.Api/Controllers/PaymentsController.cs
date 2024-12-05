using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Domain.Models.Responses;
using PaymentGateway.Api.Domain.Repositories;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly InMemoryPaymentsRepository _inMemoryPaymentsRepository;

    public PaymentsController(InMemoryPaymentsRepository inMemoryPaymentsRepository)
    {
        _inMemoryPaymentsRepository = inMemoryPaymentsRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MakePaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _inMemoryPaymentsRepository.GetById(id);
        return new OkObjectResult(payment);
    }
}