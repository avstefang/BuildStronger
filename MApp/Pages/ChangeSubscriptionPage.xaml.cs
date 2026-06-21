using MApp.PageModels;

namespace MApp.Pages;

public partial class ChangeSubscriptionPage : ContentPage
{
    private readonly ChangeSubscriptionPageModel _model;

    public ChangeSubscriptionPage(ChangeSubscriptionPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.LoadAsync();
    }
}
