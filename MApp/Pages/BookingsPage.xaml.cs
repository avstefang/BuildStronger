using MApp.PageModels;

namespace MApp.Pages;

public partial class BookingsPage : ContentPage
{
    public BookingsPage(BookingsPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
