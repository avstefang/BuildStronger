using Domain.Enum;
using Domain.Exception;
using Domain.Value_object;

namespace Domain.Entity;

public class Payment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime PayedAt { get; private set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; private set; } = PaymentMethod.Wero;
    public Subscription Subscription { get; private set; }
    public ProcessorId ProcessorId { get; private set; }
    public Currency Currency { get; private set; } = Currency.EUR;

    private Payment() { }

    public Payment(ProcessorId processorId, Subscription subscription)
    {
        ProcessorId = processorId;
        Subscription = subscription;
        Amount = Subscription.SubscriptionPlan.Price;
        Currency = Subscription.SubscriptionPlan.Currency;
        Method = Subscription.SubscriptionPlan.PaymentMethod;
    }

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