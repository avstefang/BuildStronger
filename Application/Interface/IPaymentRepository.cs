using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Enum;
using Domain.Value_object;

namespace Application.Interface;

public interface IPaymentRepository
{
    Task<Payment> GetPaymentByIdAsync(Guid paymentId);
    Task<IEnumerable<Payment>> GetPaymentByEmailAsync(EmailAddress email);
    Task<PaymentStatus> CreatePaymentAsync(Payment payment);
    Task<bool> VerifyPaymentAsync(Guid paymentId);
}
