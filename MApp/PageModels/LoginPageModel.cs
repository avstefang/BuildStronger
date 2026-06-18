using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Application.Dto;

namespace MApp.PageModels;

public partial class LoginPageModel(EntityManager<LoginResponseDto, LoginAthleteDto> loginManager, IAuthService auth) : ObservableObject
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

    [RelayCommand]
    private async Task Login()
    {
        ErrorMessage = null;
        IsBusy = true;

        LoginAthleteDto login = new(Email, Password);
        LoginResponseDto? loginResponse;
        try
        {
            loginResponse = await _loginManager.PostAsync("Auth/login", login);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Een fout is opgetreden tijdens het inloggen: {ex.Message}";
            IsBusy = false;
            return;
        }

        if (loginResponse == null)
        {
            ErrorMessage = "Inloggen mislukt. Controleer uw inloggegevens en probeer het opnieuw.";
            IsBusy= false;
            return;
        }


        await _auth.SaveTokenAsync(loginResponse.Token);
        IsBusy = false;
        await Shell.Current.GoToAsync("//home");
    }

    [RelayCommand]
    private Task GoToRegister() => Shell.Current.GoToAsync("register");
}
