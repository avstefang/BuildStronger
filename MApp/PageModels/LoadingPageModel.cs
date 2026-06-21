using CommunityToolkit.Mvvm.ComponentModel;

namespace MApp.PageModels;

public partial class LoadingPageModel(IAuthService auth) : ObservableObject
{
    private readonly IAuthService _auth = auth;

    /// <summary>
    /// Decides the entry point: a valid token goes straight to the app, otherwise login.
    /// Absolute routes ("//...") reset the navigation stack so the loading page is not
    /// left on the back stack.
    /// </summary>
    public async Task NavigateBasedOnAuthAsync()
    {
        string route = await _auth.IsAuthenticatedAsync() ? "//home" : "//login";
        await Shell.Current.GoToAsync(route);
    }
}
