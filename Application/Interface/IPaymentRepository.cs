using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment> GetPaymentAsync(Payment payment);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task AddPaymentAsync(Payment payment);
    Task UpdatePaymentAsync(Payment payment);
    Task DeletePaymentAsync(Payment payment);
}
