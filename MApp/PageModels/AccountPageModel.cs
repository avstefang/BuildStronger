using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class AccountPageModel : ObservableObject
{
    [ObservableProperty]
    private MemberProfile _profile = new();

    public AccountPageModel()
    {
        LoadMockData();
    }

    [RelayCommand]
    private Task EditProfile() => Shell.Current.GoToAsync("editprofile");

    [RelayCommand]
    private Task Logout()
    {
        // TODO: clear the stored token/session.
        // Absolute route resets the stack so Back cannot return into the app.
        return Shell.Current.GoToAsync("//login");
    }

    private void LoadMockData()
    {
        // TODO: replace mock data with the logged-in member's profile from the API.
        Profile = new MemberProfile
        {
            FullName = "Anna de Vries",
            Email = "anna@example.com",
            Username = "anna_dv",
            PlanName = "Unlimited — monthly",
            Status = "Active",
            StartDate = DateTime.Today.AddMonths(-3),
            EndDate = DateTime.Today.AddMonths(1),
        };
    }
}
