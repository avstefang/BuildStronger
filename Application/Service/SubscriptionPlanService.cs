using Application.Dto;
using Application.Interface;
using Application.Mapping;

namespace Application.Service;

public class SubscriptionPlanService(ISubscriptionPlanRepository subscriptionPlanRepository)
{
    private readonly ISubscriptionPlanRepository _subscriptionPlanRepository = subscriptionPlanRepository;
    public async Task<IEnumerable<GetSubscriptionPlanDto>> GetAllSubscriptionPlansAsync() =>
        (await _subscriptionPlanRepository.GetAllSubscriptionPlansAsync())?.Select(plan => plan.ToDto()) ?? [];
    public async Task<GetSubscriptionPlanDto?> GetSubscriptionPlanByIdAsync(Guid id) =>
        (await _subscriptionPlanRepository.GetSubscriptionPlanByIdAsync(id))?.ToDto();
}