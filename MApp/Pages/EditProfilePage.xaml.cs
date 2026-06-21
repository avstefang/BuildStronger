using MApp.PageModels;

namespace MApp.Pages;

public partial class EditProfilePage : ContentPage
{
    private readonly EditProfilePageModel _model;

    public EditProfilePage(EditProfilePageModel model)
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
