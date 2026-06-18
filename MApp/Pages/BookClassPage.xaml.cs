using MApp.PageModels;

namespace MApp.Pages;

public partial class BookClassPage : ContentPage
{
    public BookClassPage(BookClassPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
