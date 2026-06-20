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
    EntityManager<GetBookedSpotDto, object> spotManager,
    EntityManager<GetCreditStatusDto, object> creditManager,
    EntityManager<GetLessonParticipantDto, object> participantManager,
    PhotoService photoService,
    IAuthService auth,
    LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetLessonDto, string> _lessonManager = lessonManager;
    private readonly EntityManager<GetReservationDto, AddReservationDto> _reservationManager = reservationManager;
    private readonly EntityManager<GetBookedSpotDto, object> _spotManager = spotManager;
    private readonly EntityManager<GetCreditStatusDto, object> _creditManager = creditManager;
    private readonly EntityManager<GetLessonParticipantDto, object> _participantManager = participantManager;
    private readonly PhotoService _photoService = photoService;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    private Guid _lessonId;

    /// <summary>The lesson's id, passed as a string through the navigation query.</summary>
    public string LessonId { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanConfirm))]
    private bool _hasCredits = true;

    /// <summary>The confirmed members attending this lesson (username + photo).</summary>
    [ObservableProperty]
    private ObservableCollection<Participant> _participants = [];

    [ObservableProperty]
    private bool _hasParticipants;

    /// <summary>Spinning requires a chosen bike; other classes can be booked directly. Bookable until 1h
    /// before and only while the member has credits left this period.</summary>
    public bool CanConfirm => !IsBusy
        && (!IsSpinning || SelectedBike is not null)
        && SelectedClass is not null
        && SelectedClass.Start > DateTime.Now.AddHours(1)
        && SelectedClass.Start <= DateTime.Now.Add(GymClass.BookingWindow)
        && HasCredits;

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
            Bikes = IsSpinning ? await BuildBikeGridAsync(token) : [];

            await CheckCreditsAsync(token);
            await LoadParticipantsAsync(token);
        }
        catch
        {
            ErrorMessage = "Kon de les niet laden. Probeer het opnieuw.";
        }
    }

    // Loads the confirmed members for this lesson and downloads their profile photos into the cache.
    // Best-effort: a failure here just hides the list rather than blocking the booking screen.
    private async Task LoadParticipantsAsync(string token)
    {
        try
        {
            var dtos = await _participantManager.GetEntitiesAsync($"Reservation/lesson/{_lessonId}/participants", token) ?? [];
            var list = new ObservableCollection<Participant>();
            foreach (var dto in dtos)
            {
                var participant = new Participant { Username = dto.Username };
                list.Add(participant);

                // Resolve the avatar after adding, so the row shows immediately and the image
                // fills in once downloaded (PhotoPath is observable).
                if (!string.IsNullOrEmpty(dto.PhotoFile))
                    participant.PhotoPath = await _photoService.DownloadToCacheAsync(token, dto.PhotoFile);
            }

            Participants = list;
            HasParticipants = list.Count > 0;
        }
        catch
        {
            Participants = [];
            HasParticipants = false;
        }
    }

    // Disables booking (with a message) when the member is out of credits or has no active subscription.
    // The server enforces this too; this just makes the hard stop visible up front.
    private async Task CheckCreditsAsync(string token)
    {
        try
        {
            GetCreditStatusDto? credits = await _creditManager.GetEntityAsync("Reservation/credits", token);
            HasCredits = credits is not null && (credits.Unlimited || credits.Remaining > 0);
            if (!HasCredits)
                ErrorMessage = "Je hebt geen credits meer over voor deze periode.";
        }
        catch
        {
            // On a lookup failure, let the server be the gate rather than blocking the screen.
            HasCredits = true;
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

        // Backstop for the 1-hour rule in case the cutoff passed while the page was open.
        if (SelectedClass.Start <= DateTime.Now.AddHours(1))
        {
            ErrorMessage = "Je kunt tot uiterlijk 1 uur van tevoren boeken.";
            return;
        }

        // Reservations are limited to a week ahead (the server enforces this too).
        if (SelectedClass.Start > DateTime.Now.Add(GymClass.BookingWindow))
        {
            ErrorMessage = "Je kunt tot maximaal een week van tevoren boeken.";
            return;
        }

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

            // Spinning classes book a specific bike by its grid position (row/column);
            // other classes are booked without a spot.
            AddReservationDto dto = IsSpinning && SelectedBike is not null
                ? new(athlete.Email, _lessonId, SelectedClass.Start, SelectedBike.Row, SelectedBike.Column)
                : new(athlete.Email, _lessonId, SelectedClass.Start);

            await _reservationManager.CreateEntityAsync("Reservation", dto, token);

            // The bookings list changed — drop its cache so the Bookings page re-fetches.
            await _localDb.ClearBookingsAsync();
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Reserveren is mislukt. Probeer het opnieuw. ({ex.Message})";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    // Spinning room: 4 rows of 6 bikes. Bikes already booked for this lesson are marked taken.
    private async Task<ObservableCollection<BikeSpot>> BuildBikeGridAsync(string token)
    {
        HashSet<(int Row, int Column)> taken = await GetTakenPositionsAsync(token);
        var bikes = new ObservableCollection<BikeSpot>();
        var number = 1;
        for (var row = 1; row <= 4; row++)
        {
            for (var column = 1; column <= 6; column++)
            {
                bikes.Add(new BikeSpot { Number = number, Row = row, Column = column, IsTaken = taken.Contains((row, column)) });
                number++;
            }
        }
        return bikes;
    }

    // Fetches the spots already booked for this lesson, keyed by (row, column) to match the grid.
    private async Task<HashSet<(int Row, int Column)>> GetTakenPositionsAsync(string token)
    {
        try
        {
            var booked = await _spotManager.GetEntitiesAsync($"Reservation/lesson/{_lessonId}/spots", token) ?? [];
            return booked.Select(b => (b.RowNumber, b.SpotNumber)).ToHashSet();
        }
        catch
        {
            // If the lookup fails, show all bikes as free rather than blocking the booking screen.
            return [];
        }
    }
}
