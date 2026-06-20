using CommunityToolkit.Mvvm.ComponentModel;

namespace MApp.Models;

/// <summary>A fellow member attending a lesson, shown on the booking screen (username + photo only).</summary>
public partial class Participant : ObservableObject
{
    public string Username { get; set; } = string.Empty;

    // The photo is downloaded asynchronously after the list is shown, so this is observable
    // and HasPhoto re-evaluates once the cached file path arrives.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPhoto))]
    private string? _photoPath;

    public bool HasPhoto => !string.IsNullOrEmpty(PhotoPath);
}
