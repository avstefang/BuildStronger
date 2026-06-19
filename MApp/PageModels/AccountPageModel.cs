using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;
using Brand.Theme;

namespace MApp.PageModels;

public partial class AccountPageModel : ObservableObject
{
    private IAuthService _authService;
    private LocalDbService _localDb;

    [ObservableProperty]
    private MemberProfile _profile = new();

    [ObservableProperty]
    private bool _hasSubscription;

    [ObservableProperty]
    private string _color = BrandColors.Success;

    public AccountPageModel(IAuthService authService, LocalDbService localDb)
    {
        _authService = authService;
        _localDb = localDb;
    }

    [RelayCommand]
    private Task EditProfile() => Shell.Current.GoToAsync("editprofile");

    [RelayCommand]
    private Task AddSubscription() => Shell.Current.GoToAsync("addsubscription");

    [RelayCommand]
    private Task ChangeSubscription() => Shell.Current.GoToAsync("changesubscription");

    [RelayCommand]
    private Task Logout()
    {
        _authService.Logout();
        return Shell.Current.GoToAsync("//login");
    }

    /// <summary>Loads the signed-in member from the local cache. Awaited from the page's OnAppearing.</summary>
    public async Task LoadProfileAsync()
    {
        MemberProfile? athlete = await _localDb.GetAthleteAsync();
        Profile = athlete ?? new MemberProfile();
        HasSubscription = Profile.Status != "Geen actief abonnement";

        Color = Profile.Status switch
        {
            "Failed" => BrandColors.Error,
            "WaitingActivation" => BrandColors.Warning,
            "Expired" => BrandColors.Attention,
            "Cancelled" => BrandColors.Warning,
            _ => BrandColors.Success
        };
    }
}
