using MApp.Models;

namespace Test.Unit.MApp;

[TestClass]
public class GymClassTests
{
    [TestMethod]
    public void IsFull_WhenNoSpotsLeft_ReturnsTrue()
    {
        var gymClass = new GymClass { SpotsLeft = 0 };
        Assert.IsTrue(gymClass.IsFull);
    }

    [TestMethod]
    public void IsFull_WhenSpotsAvailable_ReturnsFalse()
    {
        var gymClass = new GymClass { SpotsLeft = 3 };
        Assert.IsFalse(gymClass.IsFull);
    }

    [TestMethod]
    public void IsBookable_WithinWindowAndNotFull_ReturnsTrue()
    {
        var gymClass = new GymClass
        {
            SpotsLeft = 5,
            IsBooked = false,
            Start = DateTime.Now.AddHours(2)
        };
        Assert.IsTrue(gymClass.IsBookable);
    }

    [TestMethod]
    public void IsBookable_WhenFull_ReturnsFalse()
    {
        var gymClass = new GymClass
        {
            SpotsLeft = 0,
            IsBooked = false,
            Start = DateTime.Now.AddHours(2)
        };
        Assert.IsFalse(gymClass.IsBookable);
    }

    [TestMethod]
    public void IsBookable_WhenAlreadyBooked_ReturnsFalse()
    {
        var gymClass = new GymClass
        {
            SpotsLeft = 5,
            IsBooked = true,
            Start = DateTime.Now.AddHours(2)
        };
        Assert.IsFalse(gymClass.IsBookable);
    }

    [TestMethod]
    public void IsBookable_LessThanOneHourAway_ReturnsFalse()
    {
        var gymClass = new GymClass
        {
            SpotsLeft = 5,
            IsBooked = false,
            Start = DateTime.Now.AddMinutes(30)
        };
        Assert.IsFalse(gymClass.IsBookable);
    }

    [TestMethod]
    public void IsBookable_MoreThan7DaysAway_ReturnsFalse()
    {
        var gymClass = new GymClass
        {
            SpotsLeft = 5,
            IsBooked = false,
            Start = DateTime.Now.AddDays(8)
        };
        Assert.IsFalse(gymClass.IsBookable);
    }

    [TestMethod]
    public void SpotsLabel_WhenFull_ReturnsVol()
    {
        var gymClass = new GymClass { SpotsLeft = 0 };
        Assert.AreEqual("Vol", gymClass.SpotsLabel);
    }

    [TestMethod]
    public void SpotsLabel_WhenSpotsAvailable_ShowsCount()
    {
        var gymClass = new GymClass { SpotsLeft = 4 };
        Assert.AreEqual("4 plekken vrij", gymClass.SpotsLabel);
    }

    [TestMethod]
    public void NextOccurrence_ValidDay_ReturnsThatDayOfWeek()
    {
        var result = GymClass.NextOccurrence("Monday", new TimeOnly(8, 0));
        Assert.AreEqual(DayOfWeek.Monday, result.DayOfWeek);
        Assert.AreEqual(8, result.Hour);
        Assert.AreEqual(0, result.Minute);
    }

    [TestMethod]
    public void NextOccurrence_InvalidDay_FallsBackToToday()
    {
        var result = GymClass.NextOccurrence("NotADay", new TimeOnly(10, 0));
        Assert.AreEqual(DateTime.Today.Date, result.Date);
    }

    [TestMethod]
    public void BookButtonText_WhenBooked_ReturnsGeboekt()
    {
        var gymClass = new GymClass { IsBooked = true };
        Assert.AreEqual("Geboekt", gymClass.BookButtonText);
    }

    [TestMethod]
    public void BookButtonText_WhenNotBooked_ReturnsBoeken()
    {
        var gymClass = new GymClass { IsBooked = false };
        Assert.AreEqual("Boeken", gymClass.BookButtonText);
    }
}
