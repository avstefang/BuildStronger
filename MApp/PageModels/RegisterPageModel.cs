using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace MApp.PageModels;

public partial class RegisterPageModel(EntityManager<RegisterAthleteDto, string> registerManager, EntityManager<GetSubscriptionPlanDto, string> subscriptionPlanManager) : ObservableObject
{
    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    private IEnumerable<GetSubscriptionPlanDto>? _subscriptionPlanDto;

    /// <summary>Shown first so a member can create an account without subscribing yet.</summary>
    public const string NoSubscription = "No subscription for now";
    
    public ObservableCollection<string> Plans { get; private set; } = [NoSubscription];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSubscription))]
    [NotifyPropertyChangedFor(nameof(SubmitText))]
    private string _selectedPlan = NoSubscription;

    /// <summary>True once a real plan is picked; reveals start date + payment method.</summary>
    public bool HasSubscription => SelectedPlan != NoSubscription;

    public string SubmitText => HasSubscription ? "Pay & register" : "Create account";

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today;

    public DateTime MinStartDate { get; } = DateTime.Today;

    public List<string> PaymentMethods { get; } = ["iDEAL", "Wero"];

    [ObservableProperty]
    private string _selectedPaymentMethod = "iDEAL";

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    private EntityManager<RegisterAthleteDto, string> _registerManager = registerManager;
    private EntityManager<GetSubscriptionPlanDto, string> _subscriptionPlanManager = subscriptionPlanManager;

    [RelayCommand]
    private async Task Register()
    {
        try
        {
            

        }
        catch
        {

        }
        ErrorMessage = null;
        IsBusy = true;
        await Task.CompletedTask;
        IsBusy = false;

        // New member is signed in: reset the stack into the app.
        await Shell.Current.GoToAsync("//home");
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    public async Task LoadSubscriptionPlans()
    {
        _subscriptionPlanDto = await _subscriptionPlanManager.GetEntitiesNoAuthAsync("SubscriptionPlan/plans") ?? [];
        foreach (var plan in _subscriptionPlanDto)
        {
            Plans.Add(plan.Name);
        }
    }
}
