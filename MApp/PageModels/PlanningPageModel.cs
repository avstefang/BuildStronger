using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class PlanningPageModel(EntityManager<GetLessonDto, string> lessonManager, IAuthService auth) : ObservableObject
{
    private readonly EntityManager<GetLessonDto, string> _lessonManager = lessonManager;
    private readonly IAuthService _auth = auth;

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
            await LoadLessonsAsync();
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

    /// <summary>Loads the full lesson schedule from the API (ordered by start time).</summary>
    public async Task LoadLessonsAsync()
    {
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            var lessons = await _lessonManager.GetEntitiesAsync("Lesson", token) ?? [];
            Classes = [.. lessons.Select(GymClass.FromDto).OrderBy(c => c.Start)];
        }
        catch
        {
            // Leave the current list in place on failure rather than crashing the page.
        }
    }
}
