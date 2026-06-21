using MApp.Models;

namespace Test.Unit.MApp;

[TestClass]
public class BookingTests
{
    [TestMethod]
    public void CanCancel_MoreThanOneHourBeforeStart_ReturnsTrue()
    {
        var booking = new Booking { Start = DateTime.Now.AddHours(2) };
        Assert.IsTrue(booking.CanCancel);
    }

    [TestMethod]
    public void CanCancel_LessThanOneHourBeforeStart_ReturnsFalse()
    {
        var booking = new Booking { Start = DateTime.Now.AddMinutes(30) };
        Assert.IsFalse(booking.CanCancel);
    }

    [TestMethod]
    public void CanCancel_StartInThePast_ReturnsFalse()
    {
        var booking = new Booking { Start = DateTime.Now.AddHours(-1) };
        Assert.IsFalse(booking.CanCancel);
    }

    [TestMethod]
    public void IsUpcoming_FutureStart_ReturnsTrue()
    {
        var booking = new Booking { Start = DateTime.Now.AddDays(1) };
        Assert.IsTrue(booking.IsUpcoming);
    }

    [TestMethod]
    public void IsUpcoming_PastStart_ReturnsFalse()
    {
        var booking = new Booking { Start = DateTime.Now.AddDays(-1) };
        Assert.IsFalse(booking.IsUpcoming);
    }

    [TestMethod]
    public void TimeRange_ShowsStartAndEndTime()
    {
        var booking = new Booking
        {
            Start = new DateTime(2026, 6, 21, 10, 0, 0),
            DurationMinutes = 60
        };
        Assert.AreEqual("10:00 - 11:00", booking.TimeRange);
    }

    [TestMethod]
    public void End_EqualStartPlusDuration()
    {
        var booking = new Booking
        {
            Start = new DateTime(2026, 6, 21, 9, 30, 0),
            DurationMinutes = 45
        };
        Assert.AreEqual(new DateTime(2026, 6, 21, 10, 15, 0), booking.End);
    }
}
