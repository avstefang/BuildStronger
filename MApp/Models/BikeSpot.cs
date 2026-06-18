using CommunityToolkit.Mvvm.ComponentModel;

namespace MApp.Models;

/// <summary>
/// One bike in the spinning room (4 rows of 6 = 24 bikes). Selectable when booking
/// a spinning class. Mock shape — replace the "taken" state with API data later.
/// </summary>
public partial class BikeSpot : ObservableObject
{
    public int Number { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsTaken { get; set; }

    [ObservableProperty]
    private bool _isSelected;

    public string Label => $"Bike {Number}";
}
