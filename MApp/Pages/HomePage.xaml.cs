using MApp.PageModels;

namespace MApp.Pages;

public partial class HomePage : ContentPage
{
    public HomePage(HomePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
