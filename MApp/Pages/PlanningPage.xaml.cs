using MApp.PageModels;

namespace MApp.Pages;

public partial class PlanningPage : ContentPage
{
    public PlanningPage(PlanningPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
