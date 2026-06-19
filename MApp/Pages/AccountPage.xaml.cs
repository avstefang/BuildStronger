using MApp.PageModels;

namespace MApp.Pages;

public partial class AccountPage : ContentPage
{
    private readonly AccountPageModel _model;

    public AccountPage(AccountPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.LoadProfileAsync();
    }
}
