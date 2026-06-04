using Domain.Enum;
using System;

namespace Domain.Entity;

public class SubscriptionPlan(
    string name,
    decimal price,
    int durationInMonths,
    int monthlyCreditAmount,
    PaymentMethod paymentMethod = PaymentMethod.Wero,
    Currency currency = Currency.EUR
) {
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
    public decimal Price { get; private set; } = price;
    public int DurationInMonths { get; private set; } = durationInMonths;
    public int MonthlyCreditAmount { get; private set; } = monthlyCreditAmount;
    public PaymentMethod PaymentMethod { get; private set; } = paymentMethod;
    public Currency Currency { get; private set; } = currency;

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));
        Name = newName;
    }

    public void ChangePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(newPrice));
        Price = newPrice;
    }

    public void ChangeDuration(int newDurationInMonths)
    {
        if (newDurationInMonths <= 0)
            throw new ArgumentException("Duration must be greater than zero.", nameof(newDurationInMonths));
        DurationInMonths = newDurationInMonths;
    }

    public void ChangeMonthlyCreditAmount(int newMonthlyCreditAmount)
    {
        if (newMonthlyCreditAmount <= 0)
            throw new ArgumentException("Monthly credit amount must be greater than zero.", nameof(newMonthlyCreditAmount));
        MonthlyCreditAmount = newMonthlyCreditAmount;
    }
}
