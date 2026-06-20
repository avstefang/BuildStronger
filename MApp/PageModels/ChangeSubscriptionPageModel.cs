using System.Globalization;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MApp.PageModels;

public partial class ChangeSubscriptionPageModel(EntityManager<GetSubscriptionDto, object> subscriptionManager, IAuthService auth, LocalDbService localDbService) : ObservableObject
{
    private readonly EntityManager<GetSubscriptionDto, object> _subscriptionManager = subscriptionManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDbService = localDbService;

    private static readonly CultureInfo Dutch = CultureInfo.GetCultureInfo("nl-NL");

    // Guards the auto-renew switch callback so it doesn't fire an API call while loading.
    private bool _isLoading;
    private DateOnly _startDate;
    private string _rawStatus = string.Empty;

    [ObservableProperty]
    private string _planName = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private string _startDateText = string.Empty;

    [ObservableProperty]
    private string _endDateText = string.Empty;

    [ObservableProperty]
    private string _priceText = string.Empty;

    [ObservableProperty]
    private bool _autoRenew;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private bool _showCancelOption = true;

    [ObservableProperty]
    private bool _showAutomaticRenewalOption = true;

    /// <summary>Activation is offered only for a waiting subscription whose start date has arrived.</summary>
    public bool CanActivate => _rawStatus == "WaitingActivation" && _startDate <= DateOnly.FromDateTime(DateTime.Today);

    /// <summary>Loads the member's subscription. Awaited from the page's OnAppearing.</summary>
    public async Task LoadAsync()
    {
        ErrorMessage = null;
        IsBusy = true;
        _isLoading = true;
        try
        {
            MemberProfile? athlete = await _localDbService.GetAthleteAsync();

            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "Je sessie is verlopen. Log opnieuw in.";
                return;
            }

            GetSubscriptionDto? sub = await _subscriptionManager.GetEntityAsync("Subscription", token);
            if (sub is null)
                return;

            if (sub.Status == "Cancelled")
            {
                ShowCancelOption = false;
                ShowAutomaticRenewalOption = false;
            }

            PlanName = sub.SubscriptionPlan.Name;
            PriceText = FormatPrice(sub.SubscriptionPlan.Price, sub.SubscriptionPlan.Currency);
            _startDate = sub.StartDate;
            StartDateText = sub.StartDate.ToString("d MMM yyyy", Dutch);
            EndDateText = sub.EndDate.ToString("d MMM yyyy", Dutch);
            _rawStatus = sub.Status;
            StatusText = TranslateStatus(sub.Status);
            AutoRenew = sub.AutoRenew;
            OnPropertyChanged(nameof(CanActivate));

            if (null == athlete) return;

            athlete.Status = sub.Status;
            athlete.IsAutoRenewalEnabled = sub.AutoRenew;
            await _localDbService.SaveAthleteAsync(athlete);
        }
        catch
        {
            ErrorMessage = "Kon je abonnement niet laden. Probeer het opnieuw.";
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }
    }

    // Fired by the Switch; only calls the API for genuine user toggles, not the initial load.
    partial void OnAutoRenewChanged(bool value)
    {
        if (_isLoading)
            return;
        _ = UpdateAutoRenewAsync(value);
    }

    private async Task UpdateAutoRenewAsync(bool enable)
    {
        ErrorMessage = null;
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            await _subscriptionManager.UpdateEntityAsync(
                $"Subscription/autorenew/{(enable ? "true" : "false")}", null, token, false);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Automatische verlenging bijwerken is mislukt. {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CancelSubscription()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            await _subscriptionManager.UpdateEntityAsync("Subscription/cancel", new object(), token, false);
            await LoadAsync();
        }
        catch
        {
            ErrorMessage = "Abonnement opzeggen is mislukt.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Activate()
    {
        ErrorMessage = null;
        IsBusy = true;
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            await _subscriptionManager.UpdateEntityAsync("Subscription/activatewaiting", new object(), token, false);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Abonnement activeren is mislukt.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Back() => Shell.Current.GoToAsync("..");

    private static string FormatPrice(decimal price, string currency)
    {
        string symbol = currency?.ToUpperInvariant() switch
        {
            "EUR" => "€",
            "USD" => "$",
            "GBP" => "£",
            _ => currency is { Length: > 0 } ? currency.ToUpperInvariant() + " " : string.Empty
        };
        return $"{symbol}{price.ToString("0.##", CultureInfo.InvariantCulture)}";
    }

    private static string TranslateStatus(string status) => status switch
    {
        "Active" => "Actief",
        "WaitingActivation" => "Wacht op activatie",
        "Cancelled" => "Geannuleerd",
        "Expired" => "Verlopen",
        "Failed" => "Mislukt",
        _ => status
    };
}
