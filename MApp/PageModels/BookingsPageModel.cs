using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class BookingsPageModel(EntityManager<GetReservationDto, object> reservationManager, IAuthService auth) : ObservableObject
{
    private readonly EntityManager<GetReservationDto, object> _reservationManager = reservationManager;
    private readonly IAuthService _auth = auth;

    [ObservableProperty]
    private ObservableCollection<Booking> _upcomingBookings = [];

    [ObservableProperty]
    private ObservableCollection<Booking> _pastBookings = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [RelayCommand]
    private Task AddBooking() => Shell.Current.GoToAsync("//planning");

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        try
        {
            await LoadBookingsAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task CancelBooking(Booking? booking)
    {
        if (booking is null)
            return;

        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            // Cancel route is PUT api/Reservation/{id}/cancel; the body is ignored by the server.
            await _reservationManager.UpdateEntityAsync($"Reservation/{booking.ReservationId}/cancel", token, new object());
            UpcomingBookings.Remove(booking);
        }
        catch
        {
            // Leave the booking in place if the cancel call failed.
        }
    }

    /// <summary>Loads the member's reservations from the API and splits them into upcoming and past.</summary>
    public async Task LoadBookingsAsync()
    {
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            var reservations = await _reservationManager.GetEntitiesAsync("Reservation/athlete", token) ?? [];
            var bookings = reservations.Select(Booking.FromDto).ToList();

            UpcomingBookings = [.. bookings.Where(b => b.IsUpcoming).OrderBy(b => b.Start)];
            PastBookings = [.. bookings.Where(b => !b.IsUpcoming).OrderByDescending(b => b.Start)];
        }
        catch
        {
            // Leave the current lists in place on failure rather than crashing the page.
        }
    }
}
