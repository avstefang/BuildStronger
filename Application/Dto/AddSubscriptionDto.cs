namespace Application.Dto;

public class AddSubscriptionDto
{
    public string ProcessorId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public Guid SubscriptionPlanId { get; set; }
    public DateOnly? StartDate { get; set; }
    public bool AutoRenew { get; set; }

    public void ChangeStartDate(DateOnly startDate) => StartDate = startDate;
}
