using MApp.Models;

namespace Test.Unit.MApp;

[TestClass]
public class BikeSpotTests
{
    [TestMethod]
    public void Label_ReturnsCorrectString()
    {
        var spot = new BikeSpot { Number = 7 };
        Assert.AreEqual("Fiets 7", spot.Label);
    }

    [TestMethod]
    public void IsTaken_DefaultIsFalse()
    {
        var spot = new BikeSpot();
        Assert.IsFalse(spot.IsTaken);
    }

    [TestMethod]
    public void IsSelected_DefaultIsFalse()
    {
        var spot = new BikeSpot();
        Assert.IsFalse(spot.IsSelected);
    }

    [TestMethod]
    public void IsSelected_WhenSetToTrue_IsTrue()
    {
        var spot = new BikeSpot();
        spot.IsSelected = true;
        Assert.IsTrue(spot.IsSelected);
    }

    [TestMethod]
    public void IsSelected_WhenChanged_FiresPropertyChanged()
    {
        var spot = new BikeSpot();
        string? changedProperty = null;
        spot.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        spot.IsSelected = true;

        Assert.AreEqual(nameof(BikeSpot.IsSelected), changedProperty);
    }

    [TestMethod]
    public void IsTaken_WhenSet_ReflectsCorrectly()
    {
        var spot = new BikeSpot { IsTaken = true };
        Assert.IsTrue(spot.IsTaken);
    }

    [TestMethod]
    public void RowAndColumn_StoreCorrectly()
    {
        var spot = new BikeSpot { Row = 2, Column = 4 };
        Assert.AreEqual(2, spot.Row);
        Assert.AreEqual(4, spot.Column);
    }
}
