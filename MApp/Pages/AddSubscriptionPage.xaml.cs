using MApp.PageModels;

namespace MApp.Pages;

public partial class AddSubscriptionPage : ContentPage
{
    private readonly AddSubscriptionPageModel _model;

    public AddSubscriptionPage(AddSubscriptionPageModel model)
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
