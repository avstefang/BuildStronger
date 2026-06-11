using Application.Interface;
using Domain.Entity;

namespace Infrastructure;

/// <summary>
/// Simulated Stripe payment processor for testing and development purposes.
/// In production, this would integrate with the actual Stripe API.
/// </summary>
public class PaymentProcessor : IPaymentProcessor
{
    private readonly Random _random = new Random();

    /// <summary>
    /// Simulates processing a payment through Stripe.
    /// Returns a random Guid to represent the Stripe charge ID.
    /// In a real implementation, this would call Stripe API endpoints.
    /// </summary>
    /// <param name="payment">The payment object containing subscription and amount details</param>
    /// <returns>A simulated Stripe charge ID (Guid), or Guid.Empty if payment fails</returns>
    public Task<Guid> ProcessPaymentAsync(Payment payment)
    {
        // Simulate payment processing
        // In production, you would:
        // 1. Create a Stripe customer if not exists
        // 2. Create a payment intent or charge with Stripe API
        // 3. Handle the response and return the transaction ID

        // For simulation: 80% success rate
        bool isSuccessful = _random.Next(0, 100) < 80;

        if (isSuccessful)
        {
            // Return a simulated Stripe charge ID
            var chargeId = Guid.NewGuid();
            return Task.FromResult(chargeId);
        }
        else
        {
            // Return empty Guid to indicate failure
            return Task.FromResult(Guid.Empty);
        }
    }
}
