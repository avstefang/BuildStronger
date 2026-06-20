using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class BookingsPageModel(EntityManager<GetReservationDto, object> reservationManager, IAuthService auth, LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetReservationDto, object> _reservationManager = reservationManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    [ObservableProperty]
    private ObservableCollection<Booking> _upcomingBookings = [];

    [ObservableProperty]
    private bool _hasUpcomingBookings = false;

    [ObservableProperty]
    private ObservableCollection<Booking> _pastBookings = [];

    [ObservableProperty]
    private bool _hasPastBookings = false;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private ObservableCollection<Booking> _waitlistBookings = [];

    [ObservableProperty]
    private bool _hasWaitlistBookings = false;

    [RelayCommand]
    private Task AddBooking() => Shell.Current.GoToAsync("//planning");

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        try
        {
            await LoadBookingsAsync(forceRefresh: true);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task CancelBooking(Booking? booking)
    {
        if (booking is null || !booking.CanCancel)
            return;

        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            // Cancel route is PUT api/Reservation/{id}/cancel and returns an empty 200, so use the
            // no-response variant (deserializing the empty body would throw and abort the removal).
            await _reservationManager.UpdateEntityAsync($"Reservation/{booking.ReservationId}/cancel", new object(), token, false);

            // Force a refresh so the cancelled booking disappears and the local cache stays in sync.
            await LoadBookingsAsync(forceRefresh: true);
        }
        catch
        {
            // Leave the booking in place if the cancel call failed.
        }
    }

    /// <summary>
    /// Loads the member's bookings. After the first load it reads from the local database; a booking,
    /// a cancel, or pull-to-refresh (forceRefresh) re-fetches from the API and updates the local copy.
    /// </summary>
    public async Task LoadBookingsAsync(bool forceRefresh = false)
    {
        try
        {
            if (!forceRefresh)
            {
                var cached = await _localDb.GetBookingsAsync();
                if (cached.Count > 0)
                {
                    ShowBookings(cached.Select(c => c.ToBooking()));
                    return;
                }
            }

            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            var reservations = await _reservationManager.GetEntitiesAsync("Reservation/athlete", token) ?? [];
            var bookings = reservations.Select(Booking.FromDto).ToList();
            await _localDb.SaveBookingsAsync(bookings.Select(CachedBooking.FromBooking));

            ShowBookings(bookings);
        }
        catch
        {
            // Leave the current lists in place on failure rather than crashing the page.
        }
    }

    // Splits the bookings into the three sections the page binds to.
    private void ShowBookings(IEnumerable<Booking> source)
    {
        var bookings = source.ToList();

        UpcomingBookings = [.. bookings.Where(b => b.IsUpcoming && b.Status != "Wachtlijst").OrderBy(b => b.Start)];
        HasUpcomingBookings = UpcomingBookings.Any();
        PastBookings = [.. bookings.Where(b => !b.IsUpcoming).OrderByDescending(b => b.Start)];
        HasPastBookings = PastBookings.Any();
        WaitlistBookings = [.. bookings.Where(b => b.Status == "Wachtlijst" && b.IsUpcoming).OrderBy(b => b.Start)];
        HasWaitlistBookings = WaitlistBookings.Any();
    }
}
