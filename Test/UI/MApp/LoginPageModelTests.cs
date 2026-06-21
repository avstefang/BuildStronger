using Application.Dto;
using MApp.PageModels;
using MApp.Services;
using Microsoft.Extensions.Configuration;

namespace Test.UI.MApp;

[TestClass]
public class LoginPageModelTests
{
    private static LoginPageModel CreateModel()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Api:Endpoint"] = "http://fake-api" })
            .Build();

        return new LoginPageModel(
            new EntityManager<LoginResponseDto, LoginAthleteDto>(config),
            new FakeAuthService(),
            new LocalDbService(),
            new EntityManager<GetSubscriptionDto, string>(config));
    }

    [TestMethod]
    public void InitialState_EmailAndPasswordAreEmpty()
    {
        var model = CreateModel();

        Assert.AreEqual(string.Empty, model.Email);
        Assert.AreEqual(string.Empty, model.Password);
    }

    [TestMethod]
    public void InitialState_IsBusyFalseAndNoErrorMessage()
    {
        var model = CreateModel();

        Assert.IsFalse(model.IsBusy);
        Assert.IsNull(model.ErrorMessage);
    }

    [TestMethod]
    public void Email_WhenSet_FiresPropertyChanged()
    {
        var model = CreateModel();
        string? changedProperty = null;
        model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        model.Email = "athlete@test.com";

        Assert.AreEqual(nameof(model.Email), changedProperty);
        Assert.AreEqual("athlete@test.com", model.Email);
    }

    [TestMethod]
    public void Password_WhenSet_FiresPropertyChanged()
    {
        var model = CreateModel();
        string? changedProperty = null;
        model.PropertyChanged += (_, e) => changedProperty = e.PropertyName;

        model.Password = "secret";

        Assert.AreEqual(nameof(model.Password), changedProperty);
        Assert.AreEqual("secret", model.Password);
    }
}
