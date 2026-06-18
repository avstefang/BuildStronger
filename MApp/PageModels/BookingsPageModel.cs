using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class BookingsPageModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Booking> _upcomingBookings = [];

    [ObservableProperty]
    private ObservableCollection<Booking> _pastBookings = [];

    [ObservableProperty]
    private bool _isRefreshing;

    public BookingsPageModel()
    {
        LoadMockData();
    }

    [RelayCommand]
    private async Task Refresh()
    {
        // TODO: reload the member's reservations from the API.
        IsRefreshing = true;
        await Task.CompletedTask;
        IsRefreshing = false;
    }

    [RelayCommand]
    private Task CancelBooking(Booking? booking)
    {
        // TODO: call the API to cancel the reservation, then refresh the list.
        if (booking is not null)
            UpcomingBookings.Remove(booking);
        return Task.CompletedTask;
    }

    private void LoadMockData()
    {
        // TODO: replace mock data with the member's reservations from the API.
        var today = DateTime.Today;
        UpcomingBookings =
        [
            new() { Id = 1, ClassName = "Spinning", Room = "Spinning room", Instructor = "Lisa van der Berg", Start = today.AddDays(1).AddHours(7), DurationMinutes = 45, SpotLabel = "Bike 14" },
            new() { Id = 2, ClassName = "Yoga", Room = "Room 1", Instructor = "Mark Jansen", Start = today.AddDays(2).AddHours(9), DurationMinutes = 60 },
        ];

        PastBookings =
        [
            new() { Id = 3, ClassName = "Bootcamp", Room = "Outdoor", Instructor = "TBD", Start = today.AddDays(-2).AddHours(10), DurationMinutes = 50, Status = "Attended" },
            new() { Id = 4, ClassName = "Boxing", Room = "Outdoor", Instructor = "TBD", Start = today.AddDays(-5).AddHours(18), DurationMinutes = 60, Status = "Attended" },
        ];
    }
}
