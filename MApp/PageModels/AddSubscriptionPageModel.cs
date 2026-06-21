using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MApp.PageModels;

public partial class AddSubscriptionPageModel(EntityManager<GetSubscriptionPlanDto, string> planManager, LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetSubscriptionPlanDto, string> _planManager = planManager;
    private readonly LocalDbService _localDb = localDb;

    private List<CachedSubscriptionPlan> _cachedPlans = [];

    public const string NoSelection = "Kies een abonnement";

    public ObservableCollection<string> Plans { get; } = [NoSelection];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelection))]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string _selectedPlan = NoSelection;

    public ObservableCollection<string> PaymentMethods { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanContinue))]
    private string _selectedPaymentMethod = string.Empty;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today;

    public DateTime MinStartDate { get; } = DateTime.Today;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>True once a real plan is picked; reveals start date + payment method.</summary>
    public bool HasSelection => SelectedPlan != NoSelection;

    public bool CanContinue => HasSelection && !string.IsNullOrWhiteSpace(SelectedPaymentMethod);

    /// <summary>Loads the plans (cache first, then API), like the registration page does.</summary>
    public async Task LoadAsync()
    {
        ErrorMessage = null;
        IsLoading = true;
        try
        {
            var cached = await _localDb.GetSubscriptionPlansAsync();
            if (cached.Count > 0)
            {
                _cachedPlans = cached;
                RebuildPlans();
                IsLoading = false;
            }

            var dto = await _planManager.GetEntitiesAsync("SubscriptionPlan/plans") ?? [];
            _cachedPlans = dto.Select(CachedSubscriptionPlan.FromDto).ToList();
            RebuildPlans();
            await _localDb.SaveSubscriptionPlansAsync(_cachedPlans);
        }
        catch
        {
            if (Plans.Count <= 1)
                ErrorMessage = "Kon geen abonnementen laden. Probeer het later opnieuw.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void RebuildPlans()
    {
        Plans.Clear();
        Plans.Add(NoSelection);
        foreach (var plan in _cachedPlans)
            Plans.Add(plan.Name);

        if (string.IsNullOrWhiteSpace(SelectedPlan))
            SelectedPlan = NoSelection;
    }

    partial void OnSelectedPlanChanged(string value)
    {
        // Show the payment methods that belong to the chosen plan.
        var plan = _cachedPlans.FirstOrDefault(p => p.Name == value);
        PaymentMethods.Clear();
        if (plan is null)
        {
            SelectedPaymentMethod = string.Empty;
            return;
        }

        foreach (var method in plan.PaymentMethodList)
            PaymentMethods.Add(method);
        SelectedPaymentMethod = PaymentMethods.FirstOrDefault() ?? string.Empty;
    }

    [RelayCommand]
    private async Task Continue()
    {
        var plan = _cachedPlans.FirstOrDefault(p => p.Name == SelectedPlan);
        if (plan is null)
            return;

        // Hand off to the payment page; flow=account keeps the member signed in afterwards.
        await Shell.Current.GoToAsync(
            $"payment?planId={Uri.EscapeDataString(plan.Id)}" +
            $"&method={Uri.EscapeDataString(SelectedPaymentMethod)}" +
            $"&startDate={StartDate:yyyy-MM-dd}" +
            $"&flow=account");
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");
}
