using Application.Interface;
using Domain.Entity;
using Domain.Value_object;

namespace Infrastructure;

public class PaymentProcessor : IPaymentProcessor
{
    private readonly Random _random = new Random();

    public Task<ProcessorId?> ProcessPaymentAsync(Payment payment)
    {
        // Simulate 80% success rate
        bool isSuccessful = _random.Next(0, 100) < 80;

        if (isSuccessful)
        {
            ProcessorId processorId = new($"sim_{Guid.NewGuid():N}");
            return Task.FromResult<ProcessorId?>(processorId);
        }

        return Task.FromResult<ProcessorId?>(null);
    }
}
