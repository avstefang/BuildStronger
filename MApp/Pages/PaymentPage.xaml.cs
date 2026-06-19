using MApp.PageModels;

namespace MApp.Pages;

public partial class PaymentPage : ContentPage
{
    private readonly PaymentPageModel _model;

    public PaymentPage(PaymentPageModel model)
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
