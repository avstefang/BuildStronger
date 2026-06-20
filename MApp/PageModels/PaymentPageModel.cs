using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Globalization;

namespace MApp.PageModels;

[QueryProperty(nameof(PlanIdRaw), "planId")]
[QueryProperty(nameof(MethodRaw), "method")]
[QueryProperty(nameof(StartDateRaw), "startDate")]
[QueryProperty(nameof(FlowRaw), "flow")]
public partial class PaymentPageModel(
    EntityManager<GetAthleteDto, AddSubscriptionDto> subscriptionManager,
    IAuthService auth,
    LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetAthleteDto, AddSubscriptionDto> _subscriptionManager = subscriptionManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    // Raw values arrive from the navigation query (Shell decodes them for us).
    public string PlanIdRaw { get; set; } = string.Empty;
    public string MethodRaw { get; set; } = string.Empty;
    public string StartDateRaw { get; set; } = string.Empty;

    /// <summary>"account" when a signed-in member adds a subscription; otherwise the registration flow.</summary>
    public string FlowRaw { get; set; } = string.Empty;

    private DateOnly _startDate = DateOnly.FromDateTime(DateTime.Today);

    [ObservableProperty]
    private string _planName = string.Empty;

    [ObservableProperty]
    private string _priceText = string.Empty;

    [ObservableProperty]
    private string _paymentMethodText = string.Empty;

    [ObservableProperty]
    private string _startDateText = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>Fills the order summary from the locally cached plan details.</summary>
    public async Task LoadAsync()
    {
        PaymentMethodText = MethodRaw;
        if (DateOnly.TryParse(StartDateRaw, CultureInfo.InvariantCulture, out DateOnly parsed))
            _startDate = parsed;
        StartDateText = _startDate.ToString("dd-MM-yyyy");

        var plans = await _localDb.GetSubscriptionPlansAsync();
        var plan = plans.FirstOrDefault(p => p.Id == PlanIdRaw);
        if (plan is null)
        {
            ErrorMessage = "Kon het gekozen abonnement niet vinden.";
            return;
        }

        PlanName = plan.Name;
        PriceText = $"{plan.Price:0.00} {plan.Currency.ToUpperInvariant()}";
    }

    [RelayCommand]
    private async Task Pay()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "Je sessie is verlopen. Log opnieuw in om je abonnement af te ronden.";
                await Task.Delay(5000); // give the user a moment to read the message
                return;
            }

            AddSubscriptionDto dto = new()
            {
                ProcessorId = $"app_{Guid.NewGuid():N}", // satisfies the processor's 15+ char rule
                PaymentMethod = MethodRaw,
                SubscriptionPlanId = Guid.TryParse(PlanIdRaw, out Guid id) ? id : Guid.Empty,
                StartDate = _startDate,
                AutoRenew = false
            };

            await _subscriptionManager.CreateEntityAsync("Subscription", dto, token);

            // The payment has been processed (succeeded or failed server-side).
            await FinishAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Er ging iets mis bij het verwerken van je betaling. Probeer het opnieuw. {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Cancel() => FinishAsync();

    private async Task FinishAsync()
    {
        // A signed-in member adding a subscription stays logged in and returns to the account.
        if (string.Equals(FlowRaw, "account", StringComparison.OrdinalIgnoreCase))
        {
            await Shell.Current.GoToAsync("//account");
            return;
        }

        // Registration flow: the token was obtained silently, so clear it and go to login.
        _auth.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}
