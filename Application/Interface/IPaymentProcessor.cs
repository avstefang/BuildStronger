using Domain.Entity;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IPaymentProcessor
{
    Task<ProcessorId?> ProcessPaymentAsync(Payment payment);
}