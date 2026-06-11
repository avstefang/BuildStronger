using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Entity;
using Domain.Enum;

namespace Infrastructure.Repository;

public class PaymentRepository : IPaymentRepository
{

    public Task<Payment> GetPaymentByIdAsync(Guid paymentId)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentStatus> CreatePaymentAsync(Payment payment)
    {
        throw new NotImplementedException();
    }

    public Task<bool> VerifyPaymentAsync(Guid paymentId)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Payment entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Payment>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Payment> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Payment entity)
    {
        throw new NotImplementedException();
    }
}
