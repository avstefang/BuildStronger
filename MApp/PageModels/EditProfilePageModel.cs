using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MApp.PageModels;

public partial class EditProfilePageModel : ObservableObject
{
    [ObservableProperty]
    private string _fullName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    public EditProfilePageModel()
    {
        LoadMockData();
    }

    [RelayCommand]
    private async Task Save()
    {
        // TODO: call the API to update the member's profile.
        IsBusy = true;
        await Task.CompletedTask;
        IsBusy = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    private void LoadMockData()
    {
        // TODO: load the logged-in member's current details from the API.
        FullName = "Anna de Vries";
        Email = "anna@example.com";
        Username = "anna_dv";
    }
}
