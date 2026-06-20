using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;
using Brand.Theme;

namespace MApp.PageModels;

public partial class AccountPageModel : ObservableObject
{
    private IAuthService _authService;
    private LocalDbService _localDb;
    private PhotoService _photoService;
    private EntityManager<GetCreditStatusDto, object> _creditManager;

    [ObservableProperty]
    private MemberProfile _profile = new();

    [ObservableProperty]
    private bool _hasSubscription;

    [ObservableProperty]
    private string _color = BrandColors.Success;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPhoto))]
    private string? _photoPath;

    [ObservableProperty]
    private bool _isUploadingPhoto;

    [ObservableProperty]
    private bool _isCancelled = false;

    // Booking credits for the current period (shown as a thin progress bar on the account page).
    [ObservableProperty]
    private bool _hasCreditBar;

    [ObservableProperty]
    private bool _isUnlimited;

    [ObservableProperty]
    private double _creditProgress;

    [ObservableProperty]
    private string _creditLabel = string.Empty;

    [ObservableProperty]
    private string _textEndDate = "Eindigt op";

    [ObservableProperty]
    private bool _showChangeSubscriptionButton = false;

    [ObservableProperty]
    private bool _showAddSubscriptionButton = true;

    /// <summary>True once a profile photo has been downloaded, so the avatar can replace the default icon.</summary>
    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);

    public void SwitchSubscriptionState(string state = "")
    {
        switch (state)
        {
            case "NoSubscription":
                ShowChangeSubscriptionButton = false;
                ShowAddSubscriptionButton = true;
                break;
            case "Cancelled":
                ShowChangeSubscriptionButton = false;
                ShowAddSubscriptionButton = false;
                break;
            default:
                ShowChangeSubscriptionButton = true;
                ShowAddSubscriptionButton = false;
                break;
        }
    }

    public AccountPageModel(IAuthService authService, LocalDbService localDb, PhotoService photoService, EntityManager<GetCreditStatusDto, object> creditManager)
    {
        _authService = authService;
        _localDb = localDb;
        _photoService = photoService;
        _creditManager = creditManager;
    }

    [RelayCommand]
    private async Task UploadPhoto()
    {
        if (IsUploadingPhoto)
            return;

        try
        {
            FileResult? picked = await MediaPicker.Default.PickPhotoAsync();
            if (picked is null)
                return;

            string? token = await _authService.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            IsUploadingPhoto = true;

            using Stream stream = await picked.OpenReadAsync();
            string? fileName = await _photoService.UploadAsync(token, stream, picked.FileName, picked.ContentType);
            if (string.IsNullOrEmpty(fileName))
                return;

            // Persist the new filename and refresh the displayed avatar.
            Profile.PhotoFile = fileName;
            await _localDb.SaveAthleteAsync(Profile);
            PhotoPath = await _photoService.DownloadToCacheAsync(token, fileName);
        }
        catch
        {
            // Leave the current avatar in place if picking/uploading failed.
        }
        finally
        {
            IsUploadingPhoto = false;
        }
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

        string? token = await _authService.GetTokenAsync();

        // Fetch the profile photo (sending the bearer token) if one was uploaded.
        if (!string.IsNullOrEmpty(Profile.PhotoFile) && !string.IsNullOrWhiteSpace(token))
            PhotoPath = await _photoService.DownloadToCacheAsync(token, Profile.PhotoFile);

        if (!string.IsNullOrWhiteSpace(token))
            await LoadCreditsAsync(token);

        if (Profile.Status == "Cancelled")
        {
            IsCancelled = true;
            SwitchSubscriptionState(Profile.EndDate < DateTime.Now ? "NoSubscription" : "Cancelled");
        }
        else if (Profile.Status == "Active" || Profile.Status == "WaitingActivation")
        {
            IsCancelled = false;
            SwitchSubscriptionState();
        }
        else // No active subscription, or an unknown status.
        {
            IsCancelled = false;
            SwitchSubscriptionState("NoSubscription");
        }

        if (Profile.IsAutoRenewalEnabled)
        {
            TextEndDate = "Verloopt op";
        }

        Color = Profile.Status switch
        {
            "Failed" => BrandColors.Error,
            "WaitingActivation" => BrandColors.Warning,
            "Expired" => BrandColors.Attention,
            "Cancelled" => BrandColors.Warning,
            _ => BrandColors.Success
        };
    }

    // Loads the credit status; shows the bar for limited plans, "Onbeperkt" for unlimited, nothing otherwise.
    private async Task LoadCreditsAsync(string token)
    {
        try
        {
            GetCreditStatusDto? credits = await _creditManager.GetEntityAsync("Reservation/credits", token);
            if (credits is null)
            {
                HasCreditBar = false;
                IsUnlimited = false;
                return;
            }

            if (credits.Unlimited)
            {
                IsUnlimited = true;
                HasCreditBar = false;
                CreditLabel = "Onbeperkt";
                return;
            }

            IsUnlimited = false;
            HasCreditBar = true;
            CreditProgress = credits.Total > 0 ? (double)credits.Remaining / credits.Total : 0;
            CreditLabel = $"{credits.Remaining} van {credits.Total} credits over";
        }
        catch
        {
            HasCreditBar = false;
        }
    }
}
