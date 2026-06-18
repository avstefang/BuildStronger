using MApp.PageModels;

namespace MApp.Pages;

public partial class EditProfilePage : ContentPage
{
    public EditProfilePage(EditProfilePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
