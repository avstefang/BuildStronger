namespace Domain.Exception;

public class PaymentFailedException : System.Exception
{
    public Guid? PaymentId { get; } = null;
    public PaymentFailedException() { }
    public PaymentFailedException(string message) : base(message) { }
    public PaymentFailedException(string message, Guid paymentId) : base(message)
    {
        PaymentId = paymentId;
    }
}