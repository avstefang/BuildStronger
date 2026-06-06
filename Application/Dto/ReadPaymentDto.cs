using Domain.Entity;
using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto;

public class ReadPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Wero;
    public required Subscription Subscription { get; set; }
    public Currency Currency { get; set; } = Currency.EUR;
    
    public void IsSuccessful()
    {
        Status = PaymentStatus.Succeeded;
    }

    public void IsFailed()
    {
        Status = PaymentStatus.Failed;
    }

    public void IsRefunded()
    {
        Status = PaymentStatus.Refunded;
    }
    public void IsCancelled()
    {
        Status = PaymentStatus.Cancelled;
    }

    public void IsPending()
    {
        Status = PaymentStatus.Pending;
    }
}