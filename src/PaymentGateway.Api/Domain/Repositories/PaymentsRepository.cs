﻿using PaymentGateway.Api.Domain.Models.Responses;

namespace PaymentGateway.Api.Domain.Repositories;

public class PaymentsRepository
{
    public List<PostPaymentResponse> Payments = new();
    
    public void Add(PostPaymentResponse payment)
    {
        Payments.Add(payment);
    }

    public PostPaymentResponse Get(Guid id)
    {
        return Payments.FirstOrDefault(p => p.Id == id);
    }
}