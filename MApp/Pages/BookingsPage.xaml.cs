using MApp.PageModels;

namespace MApp.Pages;

public partial class BookingsPage : ContentPage
{
    private readonly BookingsPageModel _model;

    public BookingsPage(BookingsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.LoadBookingsAsync();
    }
}
