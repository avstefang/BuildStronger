using System.Collections.ObjectModel;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

[QueryProperty(nameof(LessonId), "lessonId")]
public partial class BookClassPageModel(
    EntityManager<GetLessonDto, string> lessonManager,
    EntityManager<GetReservationDto, AddReservationDto> reservationManager,
    IAuthService auth,
    LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetLessonDto, string> _lessonManager = lessonManager;
    private readonly EntityManager<GetReservationDto, AddReservationDto> _reservationManager = reservationManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    private Guid _lessonId;

    /// <summary>The lesson's id, passed as a string through the navigation query.</summary>
    public string LessonId { get; set; } = string.Empty;

    [ObservableProperty]
    private GymClass? _selectedClass;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private bool _isSpinning;

    [ObservableProperty]
    private ObservableCollection<BikeSpot> _bikes = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private BikeSpot? _selectedBike;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>Spinning requires a chosen bike; other classes can be booked directly.</summary>
    public bool CanConfirm => !IsBusy && (!IsSpinning || SelectedBike is not null);

    /// <summary>Loads the chosen lesson from the API. Awaited from the page's OnAppearing.</summary>
    public async Task LoadAsync()
    {
        ErrorMessage = null;
        if (!Guid.TryParse(LessonId, out _lessonId))
        {
            ErrorMessage = "Onbekende les.";
            return;
        }

        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                ErrorMessage = "Je sessie is verlopen. Log opnieuw in.";
                return;
            }

            // There's no get-by-id lesson endpoint, so fetch the schedule and find this one.
            var lessons = await _lessonManager.GetEntitiesAsync("Lesson", token) ?? [];
            var dto = lessons.FirstOrDefault(l => l.Id == _lessonId);
            if (dto is null)
            {
                ErrorMessage = "Deze les is niet meer beschikbaar.";
                return;
            }

            SelectedClass = GymClass.FromDto(dto);
            IsSpinning = SelectedClass.IsSpinning;
            SelectedBike = null;
            Bikes = IsSpinning ? BuildBikeGrid() : [];
        }
        catch
        {
            ErrorMessage = "Kon de les niet laden. Probeer het opnieuw.";
        }
    }

    [RelayCommand]
    private void SelectBike(BikeSpot? bike)
    {
        if (bike is null || bike.IsTaken)
            return;

        // Single selection: clear the previous choice, then mark the new one.
        if (SelectedBike is not null)
            SelectedBike.IsSelected = false;

        bike.IsSelected = true;
        SelectedBike = bike;
    }

    [RelayCommand]
    private async Task Confirm()
    {
        if (SelectedClass is null)
            return;

        ErrorMessage = null;
        IsBusy = true;
        try
        {
            string? token = await _auth.GetTokenAsync();
            MemberProfile? athlete = await _localDb.GetAthleteAsync();
            if (string.IsNullOrWhiteSpace(token) || athlete is null)
            {
                ErrorMessage = "Je sessie is verlopen. Log opnieuw in.";
                return;
            }

            // The API has no equipment-spot endpoint, so the chosen bike is visual only
            // for now and the reservation is created without a specific spot id.
            AddReservationDto dto = new(athlete.Email, _lessonId, SelectedClass.Start);

            await _reservationManager.CreateEntityAsync("Reservation", token, dto);
            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            ErrorMessage = "Reserveren is mislukt. Probeer het opnieuw.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    // Spinning room: 4 rows of 6 bikes. The "taken" state is mocked until the API exposes spots.
    private static ObservableCollection<BikeSpot> BuildBikeGrid()
    {
        var taken = new HashSet<int> { 2, 5, 9, 13, 14, 20 };
        var bikes = new ObservableCollection<BikeSpot>();
        var number = 1;
        for (var row = 1; row <= 4; row++)
        {
            for (var column = 1; column <= 6; column++)
            {
                bikes.Add(new BikeSpot { Number = number, Row = row, Column = column, IsTaken = taken.Contains(number) });
                number++;
            }
        }
        return bikes;
    }
}
