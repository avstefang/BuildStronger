using MApp.PageModels;

namespace MApp.Pages;

public partial class AccountPage : ContentPage
{
    public AccountPage(AccountPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
