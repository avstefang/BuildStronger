using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IPaymentProcessor
{
    Task<Guid> ProcessPaymentAsync(Payment payment);
}