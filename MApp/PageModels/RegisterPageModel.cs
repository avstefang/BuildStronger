using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Toolkit.Picker;
using System.Collections.ObjectModel;

namespace MApp.PageModels;

public partial class RegisterPageModel(EntityManager<GetAthleteDto, RegisterAthleteDto> registerManager, EntityManager<GetSubscriptionPlanDto, string> subscriptionPlanManager, EntityManager<LoginResponseDto, LoginAthleteDto> loginManager, IAuthService auth, LocalDbService localDb) : ObservableObject
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
    public const string NoSubscription = "Nog geen abonnement";
    
    public ObservableCollection<string> Plans { get; private set; } = [NoSubscription];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSubscription))]
    [NotifyPropertyChangedFor(nameof(SubmitText))]
    private string _selectedPlan = NoSubscription;

    public bool HasSubscription => SelectedPlan != NoSubscription;

    public string SubmitText => HasSubscription ? "Betalen & registreren" : "Account aanmaken";

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today;

    public DateTime MinStartDate { get; } = DateTime.Today;

    public ObservableCollection<string> PaymentMethods { get; } = [];

    [ObservableProperty]
    private string _selectedPaymentMethod;

    [ObservableProperty]
    private bool _isBusy;

    /// <summary>True while the page first loads its plans; drives the full-page spinner.</summary>
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    private EntityManager<GetAthleteDto, RegisterAthleteDto> _registerManager = registerManager;
    private EntityManager<GetSubscriptionPlanDto, string> _subscriptionPlanManager = subscriptionPlanManager;
    private readonly EntityManager<LoginResponseDto, LoginAthleteDto> _loginManager = loginManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    // Full plan details kept in memory so OnSelectedPlanChanged can look up a
    // plan's payment methods synchronously, without an async DB read per selection.
    private List<CachedSubscriptionPlan> _cachedPlans = [];

    [RelayCommand]
    private async Task Register()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            RegisterAthleteDto registerDto = new()
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password
            };
            await _registerManager.CreateEntityAsync("Auth/register", registerDto);

            // No plan chosen: nothing to pay for, straight to the login screen.
            if (!HasSubscription)
            {
                IsBusy = false;
                await Shell.Current.GoToAsync("//login");
                return;
            }

            // A plan was chosen. The subscription endpoint is authorized, so sign in
            // silently to obtain a token, then lead the member to the payment page.
            LoginResponseDto? login = await _loginManager.PostAsync("Auth/login", new LoginAthleteDto(Email, Password));
            if (login is null)
            {
                ErrorMessage = "Account aangemaakt, maar inloggen lukte niet. Log in om je abonnement af te ronden.";
                IsBusy = false;
                await Shell.Current.GoToAsync("//login");
                return;
            }
            await _auth.SaveTokenAsync(login.Token);

            CachedSubscriptionPlan? plan = _cachedPlans.FirstOrDefault(p => p.Name == SelectedPlan);
            IsBusy = false;
            if (plan is null)
            {
                // Shouldn't happen, but don't strand the user if the plan vanished.
                await Shell.Current.GoToAsync("//login");
                return;
            }

            await Shell.Current.GoToAsync(
                $"payment?planId={Uri.EscapeDataString(plan.Id)}" +
                $"&method={Uri.EscapeDataString(SelectedPaymentMethod)}" +
                $"&startDate={StartDate:yyyy-MM-dd}");
        }
        catch
        {
            ErrorMessage = "Registratie mislukt. Controleer je gegevens en probeer het opnieuw.";
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    partial void OnSelectedPlanChanged(string value)
    {
        // Show the payment methods that belong to the chosen plan, split out of
        // the cached entity's comma-separated PaymentMethods string.
        var plan = _cachedPlans.FirstOrDefault(p => p.Name == value);
        RebuildPaymentMethods(plan?.PaymentMethodList);
    }

    public async Task LoadSubscriptionPlans()
    {
        ErrorMessage = null;
        IsLoading = true;
        try
        {
            // Fast path: show whatever we cached on a previous visit so the picker
            // fills instantly. The full-page spinner only stays up if the cache is empty.
            var cached = await _localDb.GetSubscriptionPlansAsync();
            if (cached.Count > 0)
            {
                _cachedPlans = cached;
                RebuildPlans(cached.Select(c => c.Name));
                IsLoading = false; // form is now usable; the API refresh below runs quietly
            }

            // Refresh from the API and update the cache for next time.
            _subscriptionPlanDto = await _subscriptionPlanManager.GetEntitiesAsync("SubscriptionPlan/plans") ?? [];
            _cachedPlans = _subscriptionPlanDto.Select(CachedSubscriptionPlan.FromDto).ToList();
            RebuildPlans(_subscriptionPlanDto.Select(p => p.Name));
            await _localDb.SaveSubscriptionPlansAsync(_cachedPlans);
        }
        catch
        {
            // Offline or API error: keep whatever the cache already gave us.
            if (Plans.Count <= 1)
                ErrorMessage = "Kon geen abonnementen laden. Probeer het later opnieuw.";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>Resets the picker to "no subscription" first, then the given plan names.</summary>
    private void RebuildPlans(IEnumerable<string> planNames)
    {
        Plans.Clear();
        Plans.Add(NoSubscription);
        foreach (var name in planNames)
            Plans.Add(name);

        if (string.IsNullOrWhiteSpace(SelectedPlan))
        {
            SelectedPlan = NoSubscription;
        }
    }

    private void RebuildPaymentMethods(IReadOnlyList<string>? paymentMethods)
    {
        PaymentMethods.Clear();

        if (paymentMethods is null || paymentMethods.Count == 0)
        {
            SelectedPaymentMethod = string.Empty;
            return;
        }

        foreach (var method in paymentMethods)
            PaymentMethods.Add(method);

        SelectedPaymentMethod = PaymentMethods.FirstOrDefault() ?? string.Empty;
    }
}
