using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class HomePageModel : ObservableObject
{
    [ObservableProperty]
    private string _memberName = "Athlete";

    [ObservableProperty]
    private ObservableCollection<GymClass> _featuredClasses = [];

    [ObservableProperty]
    private ObservableCollection<SubscriptionPlanInfo> _plans = [];

    public HomePageModel()
    {
        LoadMockData();
    }

    [RelayCommand]
    private Task ViewSchedule()
        => Shell.Current.GoToAsync("//planning");

    [RelayCommand]
    private Task ViewPlans()
        => Shell.Current.GoToAsync("//account");

    private void LoadMockData()
    {
        // TODO: replace mock data with a call to the API (public class offering).
        FeaturedClasses =
        [
            new() { Id = 1, Name = "Spinning", Room = "Spinning room", Instructor = "Lisa van der Berg", Start = DateTime.Today.AddHours(7), DurationMinutes = 45, Capacity = 24, SpotsLeft = 6, IsSpinning = true },
            new() { Id = 2, Name = "Yoga", Room = "Room 1", Instructor = "Mark Jansen", Start = DateTime.Today.AddHours(9), DurationMinutes = 60, Capacity = 42, SpotsLeft = 20 },
            new() { Id = 3, Name = "Bootcamp", Room = "Outdoor", Instructor = "TBD", Start = DateTime.Today.AddHours(10), DurationMinutes = 50, Capacity = 20, SpotsLeft = 4 },
            new() { Id = 4, Name = "Boxing", Room = "Outdoor", Instructor = "TBD", Start = DateTime.Today.AddHours(18), DurationMinutes = 60, Capacity = 20, SpotsLeft = 9 },
        ];

        Plans =
        [
            new() { Name = "2x per week", Price = "€29", Description = "Train up to twice per week, billed monthly" },
            new() { Name = "2x per week", Price = "€299", Description = "Train up to twice per week, billed yearly" },
            new() { Name = "Unlimited", Price = "€55", Description = "Unlimited training, billed monthly", Featured = true },
            new() { Name = "Unlimited", Price = "€549", Description = "Unlimited training, billed yearly" },
        ];
    }
}
