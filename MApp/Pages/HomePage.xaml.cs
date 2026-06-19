using MApp.PageModels;

namespace MApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomePageModel _model;

    public HomePage(HomePageModel model)
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
