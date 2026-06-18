using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

[QueryProperty(nameof(ClassId), "id")]
public partial class BookClassPageModel : ObservableObject
{
    [ObservableProperty]
    private int _classId;

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

    /// <summary>Spinning requires a chosen bike; other classes can be booked directly.</summary>
    public bool CanConfirm => !IsSpinning || SelectedBike is not null;

    partial void OnClassIdChanged(int value) => LoadMockData(value);

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
        // TODO: call the API to create the reservation
        //       (include SelectedBike.Number for spinning classes), then refresh bookings.
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private Task Cancel() => Shell.Current.GoToAsync("..");

    private void LoadMockData(int id)
    {
        // TODO: fetch the selected class from the API by id.
        SelectedClass = new GymClass
        {
            Id = id,
            Name = "Spinning",
            Room = "Spinning room",
            Instructor = "Lisa van der Berg",
            Start = DateTime.Today.AddDays(1).AddHours(7),
            DurationMinutes = 45,
            Capacity = 24,
            SpotsLeft = 6,
            IsSpinning = true,
        };

        IsSpinning = SelectedClass.IsSpinning;
        SelectedBike = null;

        if (!IsSpinning)
        {
            Bikes = [];
            return;
        }

        // Spinning room: 4 rows of 6 bikes. Mock a few as already taken.
        var taken = new HashSet<int> { 2, 5, 9, 13, 14, 20 };
        var bikes = new ObservableCollection<BikeSpot>();
        var number = 1;
        for (var row = 1; row <= 4; row++)
        {
            for (var column = 1; column <= 6; column++)
            {
                bikes.Add(new BikeSpot
                {
                    Number = number,
                    Row = row,
                    Column = column,
                    IsTaken = taken.Contains(number),
                });
                number++;
            }
        }
        Bikes = bikes;
    }
}
