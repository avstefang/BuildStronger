using MApp.PageModels;

namespace MApp.Pages;

public partial class PlanningPage : ContentPage
{
    private readonly PlanningPageModel _model;

    public PlanningPage(PlanningPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
        _model = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _model.LoadLessonsAsync();
    }
}
