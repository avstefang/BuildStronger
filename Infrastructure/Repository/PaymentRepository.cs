using Application.Interface;
using Domain.Entity;
using Domain.Enum;
using Domain.Value_object;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class PaymentRepository(PaymentDbContext dbContext) : Repository<Payment, Guid>(dbContext), IPaymentRepository
{
    public async Task<Payment> GetPaymentByIdAsync(Guid paymentId)
    {
        return await DbContext.Set<Payment>()
            .Include(p => p.Subscription).ThenInclude(s => s.SubscriptionPlan)
            .FirstOrDefaultAsync(p => p.Id == paymentId)
            ?? throw new InvalidOperationException($"Payment with ID {paymentId} not found.");
    }

    public async Task<IEnumerable<Payment>> GetPaymentByEmailAsync(EmailAddress email)
    {
        var athleteId = await DbContext.Set<Subscription>()
            .Where(s => EF.Property<Guid>(s, "AthleteId") != Guid.Empty)
            .Select(s => EF.Property<Guid>(s, "AthleteId"))
            .FirstOrDefaultAsync();

        return await DbContext.Set<Payment>()
            .Include(p => p.Subscription).ThenInclude(s => s.SubscriptionPlan)
            .Where(p => EF.Property<Guid>(p.Subscription, "AthleteId") == athleteId)
            .ToListAsync();
    }

    public async Task<PaymentStatus> CreatePaymentAsync(Payment payment)
    {
        if (DbContext.Entry(payment.Subscription).State == EntityState.Detached)
            DbContext.Attach(payment.Subscription);
        if (DbContext.Entry(payment.Subscription.SubscriptionPlan).State == EntityState.Detached)
            DbContext.Attach(payment.Subscription.SubscriptionPlan);
        await AddAsync(payment);
        return payment.Status;
    }

    public async Task<bool> VerifyPaymentAsync(Guid paymentId)
    {
        Payment payment = await GetPaymentByIdAsync(paymentId);
        return payment.Status == PaymentStatus.Succeeded;
    }
}
