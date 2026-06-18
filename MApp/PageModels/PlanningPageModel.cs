using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class PlanningPageModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<GymClass> _classes = [];

    [ObservableProperty]
    private bool _isRefreshing;

    public PlanningPageModel()
    {
        LoadMockData();
    }

    [RelayCommand]
    private async Task Refresh()
    {
        // TODO: reload the schedule from the API.
        IsRefreshing = true;
        await Task.CompletedTask;
        IsRefreshing = false;
    }

    [RelayCommand]
    private Task BookClass(GymClass? gymClass)
        => gymClass is null
            ? Task.CompletedTask
            : Shell.Current.GoToAsync($"bookclass?id={gymClass.Id}");

    private void LoadMockData()
    {
        // TODO: replace mock data with the lesson schedule from the API.
        var today = DateTime.Today;
        Classes =
        [
            new() { Id = 1, Name = "Spinning", Room = "Spinning room", Instructor = "Lisa van der Berg", Start = today.AddHours(7), DurationMinutes = 45, Capacity = 24, SpotsLeft = 6, IsSpinning = true },
            new() { Id = 2, Name = "Yoga", Room = "Room 1", Instructor = "Mark Jansen", Start = today.AddHours(9), DurationMinutes = 60, Capacity = 42, SpotsLeft = 20 },
            new() { Id = 3, Name = "Bootcamp", Room = "Outdoor", Instructor = "TBD", Start = today.AddHours(10), DurationMinutes = 50, Capacity = 20, SpotsLeft = 0 },
            new() { Id = 4, Name = "Club Power", Room = "Room 2", Instructor = "Lisa van der Berg", Start = today.AddDays(1).AddHours(18), DurationMinutes = 50, Capacity = 32, SpotsLeft = 14 },
            new() { Id = 5, Name = "XCO", Room = "Room 3", Instructor = "Mark Jansen", Start = today.AddDays(1).AddHours(19), DurationMinutes = 45, Capacity = 24, SpotsLeft = 11 },
            new() { Id = 6, Name = "Spinning", Room = "Spinning room", Instructor = "Lisa van der Berg", Start = today.AddDays(2).AddHours(7), DurationMinutes = 45, Capacity = 24, SpotsLeft = 18, IsSpinning = true },
            new() { Id = 7, Name = "Total Body Workout", Room = "Room 3", Instructor = "TBD", Start = today.AddDays(2).AddHours(12), DurationMinutes = 60, Capacity = 24, SpotsLeft = 8 },
        ];
    }
}
