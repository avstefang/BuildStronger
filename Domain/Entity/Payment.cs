using Domain.Enum;
using Domain.Exception;

namespace Domain.Entity;

public class Payment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime PayedAt { get; private set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; private set; } = PaymentMethod.Wero;
    public Subscription Subscription { get; private set; }
    public Guid ProcessorId { get; private set; }
    public Currency Currency { get; private set; } = Currency.EUR;

    public Payment(Subscription subscription)
    {
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

    public void SetProcessorId(Guid processorId)
    {
        if (ProcessorId != Guid.Empty)
        {
            throw new PaymentFailedException("Processor ID cannot be overwritten once it's set");
        }

        if (ProcessorId == Guid.Empty)
        {
            throw new PaymentFailedException("Processor ID cannot be empty");
        }

        ProcessorId = processorId;
    }
}