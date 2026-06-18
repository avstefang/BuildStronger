using MApp.PageModels;

namespace MApp.Pages;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterPageModel _model;
    public RegisterPage(RegisterPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.LoadSubscriptionPlans();
    }
}
