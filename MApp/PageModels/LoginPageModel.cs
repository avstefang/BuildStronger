using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Application.Dto;

namespace MApp.PageModels;

public partial class LoginPageModel(EntityManager<LoginResponseDto, LoginAthleteDto> loginManager, IAuthService auth, LocalDbService localDb, EntityManager<GetSubscriptionDto, string> subscriptionManager) : ObservableObject
{
    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    private readonly EntityManager<LoginResponseDto, LoginAthleteDto> _loginManager = loginManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;
    private readonly EntityManager<GetSubscriptionDto, string> _subscriptionManager = subscriptionManager;
    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = null;
        IsBusy = true;

        LoginAthleteDto login = new(Email, Password);
        LoginResponseDto? loginResponse;
        object? subscriptionResponse;
        try
        {
            loginResponse = await _loginManager.PostAsync("Auth/login", login);
            subscriptionResponse = await _subscriptionManager.GetEntityAsync($"Subscription", loginResponse?.Token ?? string.Empty);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Een fout is opgetreden tijdens het inloggen: {ex.Message}";
            IsBusy = false;
            return;
        }

        GetSubscriptionDto? subscription;
        try
        {
            subscription = (GetSubscriptionDto?)subscriptionResponse;
        }
        catch (Exception ex)
        {
            subscription = null;
        }

        if (loginResponse == null)
        {
            ErrorMessage = "Inloggen mislukt. Controleer uw inloggegevens en probeer het opnieuw.";
            IsBusy= false;
            return;
        }

        MemberProfile athlete = new()
        {
            Id = loginResponse.Athlete.Id,
            Email = loginResponse.Athlete.Email,
            FirstName = loginResponse.Athlete.FirstName,
            LastName = loginResponse.Athlete.LastName,
            Username = loginResponse.Athlete.Username,
            Role = loginResponse.Athlete.Role,
            PlanName = subscription?.SubscriptionPlan.Name ?? string.Empty,
            Status = subscription?.Status ?? "Geen actief abonnement",
            StartDate = subscription?.StartDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue,
            EndDate = subscription?.EndDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.MinValue
        };

        await _localDb.SaveAthleteAsync(athlete);

        await _auth.SaveTokenAsync(loginResponse.Token);
        IsBusy = false;
        await Shell.Current.GoToAsync("//home");
    }

    [RelayCommand]
    private Task GoToRegister() => Shell.Current.GoToAsync("register");
}
