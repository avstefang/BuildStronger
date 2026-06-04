using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;
using Domain.Enum;

namespace Application.Interface;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> RetrievePaymentAsync(Payment payment);
    Task<PaymentStatus> CreatePaymentAsync(Payment payment);
    Task<bool> VerifyPaymentAsync(Payment payment);
}
