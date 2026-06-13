using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Enum;

namespace Application.Interface;

public interface IPaymentRepository
{
    Task<Payment> GetPaymentByIdAsync(Guid paymentId);
    Task<PaymentStatus> CreatePaymentAsync(Payment payment);
    Task<bool> VerifyPaymentAsync(Guid paymentId);
}
