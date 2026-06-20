using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MApp.PageModels;

public partial class EditProfilePageModel : ObservableObject
{
    [ObservableProperty]
    private string _firstname = string.Empty;

    [ObservableProperty]
    private string _lastname = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    private readonly LocalDbService _localDb;
    private readonly EntityManager<GetAthleteDto, UpdateAthleteDto> _entityManager;
    private readonly IAuthService _authService;

    public EditProfilePageModel(LocalDbService localDb, EntityManager<GetAthleteDto, UpdateAthleteDto> entityManager, IAuthService authService)
    {
        _localDb = localDb;
        _entityManager = entityManager;
        _authService = authService;
        LoadProfileAsync();
    }

    [RelayCommand]
    private async Task Save()
    {
        IsBusy = true;
        try
        {
            MemberProfile? profileInfo = await _localDb.GetAthleteAsync();
            if (profileInfo is null)
            {
                ErrorMessage = "Account informatie kon niet worden geladen.";
                return;
            }

            string? token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                _authService.Logout();
                return;
            }

            UpdateAthleteDto updateDto = new(profileInfo.Id, Email, Firstname, Lastname, Username);
            GetAthleteDto? updatedAthlete = await _entityManager.UpdateEntityAsync("Athlete", updateDto, token);
            if (updatedAthlete is null)
            {
                ErrorMessage = "Account informatie kon niet worden opgeslagen.";
            }

            // Update local cache with new profile info
            MemberProfile? currentProfileInfo = await _localDb.GetAthleteAsync();
            MemberProfile updatedProfile = new()
            {
                Id = profileInfo.Id,
                FirstName = Firstname,
                LastName = Lastname,
                Email = Email,
                Username = Username,
                Role = profileInfo.Role,
                PhotoFile = profileInfo.PhotoFile,
                PlanName = currentProfileInfo?.PlanName ?? string.Empty,
                Status = currentProfileInfo?.Status ?? string.Empty,
                StartDate = currentProfileInfo?.StartDate ?? DateTime.MinValue,
                EndDate = currentProfileInfo?.EndDate ?? DateTime.MinValue
            };
            await _localDb.SaveAthleteAsync(updatedProfile);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Account informatie kon niet worden opgeslagen.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    /// <summary>Fills the form from the cached athlete. Awaited from the page's OnAppearing.</summary>
    public async Task LoadProfileAsync()
    {
        MemberProfile? profileInfo = await _localDb.GetAthleteAsync();
        Firstname = profileInfo?.FirstName ?? string.Empty;
        Lastname = profileInfo?.LastName ?? string.Empty;
        Email = profileInfo?.Email ?? string.Empty;
        Username = profileInfo?.Username ?? string.Empty;
    }
}
