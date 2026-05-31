using Domain.Enum;
using Domain.Exception;

namespace Domain.Entity;

public class Payment(decimal amount, Subscription subscription, Guid stripeId)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public decimal Amount { get; private set; } = amount;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public DateTime PayedAt { get; private set; } = DateTime.UtcNow;
    public PaymentMethod Method { get; private set; } = PaymentMethod.IDEAL;
    public Subscription Subscription { get; private set; } = subscription;
    public Guid StripeId { get; private set; } = stripeId;
    public Currency Currency { get; private set; } = Currency.EUR;

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

    public void SetAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        Amount = amount;
    }

    public void SetStatus(PaymentStatus status)
    {
        Status = status;
    }

    public void SetStripeId(Guid stripeId)
    {
        StripeId = stripeId;
    }

    public void SetCurrency(Currency currency)
    {
        Currency = currency;
    }

    public void SetPaymentMethod(PaymentMethod method)
    {
        Method = method;
    }
}