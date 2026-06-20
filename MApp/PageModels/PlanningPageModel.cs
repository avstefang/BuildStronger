using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class PlanningPageModel(
    EntityManager<GetLessonDto, string> lessonManager,
    EntityManager<GetReservationDto, object> reservationManager,
    IAuthService auth,
    LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetLessonDto, string> _lessonManager = lessonManager;
    private readonly EntityManager<GetReservationDto, object> _reservationManager = reservationManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    [ObservableProperty]
    private ObservableCollection<GymClass> _classes = [];

    [ObservableProperty]
    private bool _isRefreshing;

    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        try
        {
            await LoadLessonsAsync(forceRefresh: true);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private Task BookClass(GymClass? gymClass)
        => gymClass is null
            ? Task.CompletedTask
            : Shell.Current.GoToAsync($"bookclass?lessonId={gymClass.LessonId}");

    /// <summary>
    /// Loads the lesson schedule. After the first load it reads from the local database; pull-to-refresh
    /// (forceRefresh) re-fetches from the API and updates the local copy.
    /// </summary>
    public async Task LoadLessonsAsync(bool forceRefresh = false)
    {
        try
        {
            string? token = await _auth.GetTokenAsync();

            IEnumerable<GymClass> classes;
            var cached = forceRefresh ? [] : await _localDb.GetLessonsAsync();
            if (cached.Count > 0)
            {
                classes = cached.Select(c => c.ToGymClass());
            }
            else
            {
                if (string.IsNullOrWhiteSpace(token))
                    return;

                var lessons = await _lessonManager.GetEntitiesAsync("Lesson", token) ?? [];
                var cachedLessons = lessons.Select(CachedGymClass.FromDto).ToList();
                await _localDb.SaveLessonsAsync(cachedLessons);
                classes = cachedLessons.Select(c => c.ToGymClass());
            }

            await ShowClassesAsync(classes, token);
        }
        catch
        {
            // Leave the current list in place on failure rather than crashing the page.
        }
    }

    // Only upcoming classes, earliest first; marks the ones the member already booked so their button greys out.
    private async Task ShowClassesAsync(IEnumerable<GymClass> classes, string? token)
    {
        var upcoming = classes.Where(c => c.Start > DateTime.Now).OrderBy(c => c.Start).ToList();

        HashSet<(Guid LessonId, DateTime Date)> booked = await GetBookedKeysAsync(token);
        foreach (GymClass gymClass in upcoming)
            gymClass.IsBooked = booked.Contains((gymClass.LessonId, gymClass.Start.Date));

        Classes = [.. upcoming];
    }

    // The (lesson, date) pairs the member already has a reservation for. Empty on any failure, so a
    // lookup problem just leaves classes bookable rather than blocking the planning screen.
    private async Task<HashSet<(Guid LessonId, DateTime Date)>> GetBookedKeysAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return [];

        try
        {
            var reservations = await _reservationManager.GetEntitiesAsync("Reservation/athlete", token) ?? [];
            return reservations
                .Where(r => r.Status != "Cancelled")
                .Select(r => (r.Lesson.Id, r.ReservationDate.Date))
                .ToHashSet();
        }
        catch
        {
            return [];
        }
    }
}
