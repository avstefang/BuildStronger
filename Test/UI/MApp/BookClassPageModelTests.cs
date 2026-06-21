using Application.Dto;
using MApp.Models;
using MApp.PageModels;
using MApp.Services;
using Microsoft.Extensions.Configuration;

namespace Test.UI.MApp;

[TestClass]
public class BookClassPageModelTests
{
    private static BookClassPageModel CreateModel()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Api:Endpoint"] = "http://fake-api" })
            .Build();

        return new BookClassPageModel(
            new EntityManager<GetLessonDto, string>(config),
            new EntityManager<GetReservationDto, AddReservationDto>(config),
            new EntityManager<GetBookedSpotDto, object>(config),
            new EntityManager<GetCreditStatusDto, object>(config),
            new EntityManager<GetLessonParticipantDto, object>(config),
            new PhotoService(config),
            new FakeAuthService(),
            new LocalDbService());
    }

    // CanConfirm tests

    [TestMethod]
    public void CanConfirm_WhenNoClassSelected_IsFalse()
    {
        var model = CreateModel();

        Assert.IsFalse(model.CanConfirm);
    }

    [TestMethod]
    public void CanConfirm_WhenIsBusy_IsFalse()
    {
        var model = CreateModel();
        model.SelectedClass = new GymClass { Start = DateTime.Now.AddHours(2), SpotsLeft = 5 };
        model.IsBusy = true;

        Assert.IsFalse(model.CanConfirm);
    }

    [TestMethod]
    public void CanConfirm_WhenSpinningAndNoBikeSelected_IsFalse()
    {
        var model = CreateModel();
        model.SelectedClass = new GymClass { Start = DateTime.Now.AddHours(2), SpotsLeft = 5 };
        model.IsSpinning = true;
        model.SelectedBike = null;

        Assert.IsFalse(model.CanConfirm);
    }

    [TestMethod]
    public void CanConfirm_WhenNonSpinningClassWithCredits_IsTrue()
    {
        var model = CreateModel();
        model.SelectedClass = new GymClass { Start = DateTime.Now.AddHours(2), SpotsLeft = 5 };
        model.IsSpinning = false;
        // HasCredits defaults to true

        Assert.IsTrue(model.CanConfirm);
    }

    // SelectBike command tests

    [TestMethod]
    public void SelectBike_FreeBike_SelectsItAndSetsSelectedBike()
    {
        var model = CreateModel();
        var bike = new BikeSpot { Number = 3, Row = 1, Column = 3, IsTaken = false };

        model.SelectBikeCommand.Execute(bike);

        Assert.IsTrue(bike.IsSelected);
        Assert.AreEqual(bike, model.SelectedBike);
    }

    [TestMethod]
    public void SelectBike_TakenBike_DoesNotSelectIt()
    {
        var model = CreateModel();
        var bike = new BikeSpot { Number = 5, Row = 1, Column = 5, IsTaken = true };

        model.SelectBikeCommand.Execute(bike);

        Assert.IsFalse(bike.IsSelected);
        Assert.IsNull(model.SelectedBike);
    }

    [TestMethod]
    public void SelectBike_SwitchingBikes_DeselectedPreviousOne()
    {
        var model = CreateModel();
        var first = new BikeSpot { Number = 1, Row = 1, Column = 1, IsTaken = false };
        var second = new BikeSpot { Number = 2, Row = 1, Column = 2, IsTaken = false };

        model.SelectBikeCommand.Execute(first);
        model.SelectBikeCommand.Execute(second);

        Assert.IsFalse(first.IsSelected);
        Assert.IsTrue(second.IsSelected);
        Assert.AreEqual(second, model.SelectedBike);
    }
}
