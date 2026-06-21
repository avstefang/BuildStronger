namespace MApp.Pages;

public partial class LoadingPage : ContentPage
{
    private readonly LoadingPageModel _model;

    public LoadingPage(LoadingPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Defer: navigating from within the first page's appearing can throw on WinUI.
        // The try/catch keeps a failure (e.g. a SecureStorage hiccup) from becoming an
        // unhandled exception that kills the app — we fall back to the login screen.
        Dispatcher.Dispatch(async () =>
        {
            try
            {
                await _model.NavigateBasedOnAuthAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Startup auth check failed: {ex}");
                await Shell.Current.GoToAsync("//login");
            }
        });
    }
}
