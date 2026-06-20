using System.Globalization;
using Application.Dto;
using Microsoft.Extensions.Configuration;

namespace MApp.Services;

/// <summary>
/// Automatically checks the member in to a lesson when they're physically at the gym. Once a minute
/// (while the app is running) it checks: location is permitted, we're inside the gym's radius, and the
/// member has an accepted reservation starting within a short window. If so it calls the check-in
/// endpoint — no button, no user action. When location permission is missing this does nothing; that
/// is where the RFID fallback belongs.
/// </summary>
public class PresenceService(
    EntityManager<GetReservationDto, object> reservationManager,
    IAuthService auth,
    IConfiguration config)
{
    private readonly EntityManager<GetReservationDto, object> _reservationManager = reservationManager;
    private readonly IAuthService _auth = auth;

    // Gym geofence + check-in window, configurable via appsettings.json (with sane defaults).
    private readonly double _gymLatitude = ParseDouble(config["Gym:Latitude"], 0d);
    private readonly double _gymLongitude = ParseDouble(config["Gym:Longitude"], 0d);
    private readonly double _radiusMeters = ParseDouble(config["Gym:RadiusMeters"], 15d);
    private readonly int _windowMinutes = (int)ParseDouble(config["Gym:CheckInWindowMinutes"], 15d);

    private IDispatcherTimer? _timer;
    private bool _isChecking;

    /// <summary>Starts the once-a-minute presence check. Idempotent — safe to call repeatedly.</summary>
    public void Start()
    {
        if (_timer is not null)
            return;

        IDispatcher? dispatcher = Microsoft.Maui.Controls.Application.Current?.Dispatcher;
        if (dispatcher is null)
            return;

        _timer = dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMinutes(1);
        _timer.Tick += async (_, _) => await TryAutoCheckInAsync();
        _timer.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
        _timer = null;
    }

    /// <summary>One presence check. Also called directly on app resume so check-in isn't delayed a minute.</summary>
    public async Task TryAutoCheckInAsync()
    {
        // Skip if a previous check is still running (e.g. a slow GPS fix).
        if (_isChecking)
            return;
        _isChecking = true;

        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return; // not logged in

            // GEO requires location permission; the app requests it elsewhere. Without it we stop here
            // (RFID fallback would take over). CheckStatus does not prompt.
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                return;

            Location? current = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (current is null)
                return;

            double distanceMeters = current.CalculateDistance(
                new Location(_gymLatitude, _gymLongitude), DistanceUnits.Kilometers) * 1000;
            if (distanceMeters > _radiusMeters)
                return; // not at the gym

            var reservations = await _reservationManager.GetEntitiesAsync("Reservation/athlete", token) ?? [];
            DateTime now = DateTime.Now;
            GetReservationDto? due = reservations.FirstOrDefault(r =>
                r.Status == "Accepted" && WithinWindow(StartOf(r), now));
            if (due is null)
                return; // no lesson starting around now

            // The server validates owner + window and is a no-op once already checked in.
            await _reservationManager.UpdateEntityAsync($"Reservation/{due.Id}/checkin", new object(), token, false);
        }
        catch
        {
            // Presence detection is best-effort; never surface an error to the UI.
        }
        finally
        {
            _isChecking = false;
        }
    }

    // reservationDate is a date; combine it with the lesson's start time to get the actual start.
    private static DateTime StartOf(GetReservationDto r) =>
        r.ReservationDate.Date.Add(r.Lesson.Schedule.StartTime.ToTimeSpan());

    private bool WithinWindow(DateTime start, DateTime now) =>
        now >= start.AddMinutes(-_windowMinutes) && now <= start.AddMinutes(_windowMinutes);

    private static double ParseDouble(string? value, double fallback) =>
        double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : fallback;
}
