using MApp.PageModels;

namespace MApp.Pages;

public partial class BookClassPage : ContentPage
{
    private readonly BookClassPageModel _model;

    public BookClassPage(BookClassPageModel model)
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
