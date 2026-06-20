using System.Collections.ObjectModel;
using System.Globalization;
using Application.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MApp.Models;

namespace MApp.PageModels;

public partial class HomePageModel(EntityManager<GetSubscriptionPlanDto, string> subscriptionPlanManager, EntityManager<GetLessonDto, string> lessonManager, IAuthService auth, LocalDbService localDb) : ObservableObject
{
    private readonly EntityManager<GetSubscriptionPlanDto, string> _subscriptionPlanManager = subscriptionPlanManager;
    private readonly EntityManager<GetLessonDto, string> _lessonManager = lessonManager;
    private readonly IAuthService _auth = auth;
    private readonly LocalDbService _localDb = localDb;

    private const int FeaturedClassCount = 8;

    [ObservableProperty]
    private string _memberName = "Athlete";

    [ObservableProperty]
    private ObservableCollection<GymClass> _featuredClasses = [];

    [ObservableProperty]
    private ObservableCollection<SubscriptionPlanInfo> _plans = [];

    /// <summary>True while the subscription plans are first loaded; drives the plans spinner.</summary>
    [ObservableProperty]
    private bool _isLoadingPlans;

    /// <summary>True while the featured classes are loaded from the API.</summary>
    [ObservableProperty]
    private bool _isLoadingClasses;

    /// <summary>Loads everything the home page shows: the featured classes and the plans.</summary>
    public async Task LoadAsync()
    {
        await LoadFeaturedClassesAsync();
        await LoadSubscriptionPlansAsync();
    }

    private async Task LoadFeaturedClassesAsync()
    {
        IsLoadingClasses = true;
        try
        {
            string? token = await _auth.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return;

            var lessons = await _lessonManager.GetEntitiesAsync("Lesson", token) ?? [];
            FeaturedClasses = [.. lessons.Select(GymClass.FromDto).OrderBy(c => c.Start).Take(FeaturedClassCount)];
        }
        catch
        {
            // Leave the list empty on failure rather than crashing the home page.
        }
        finally
        {
            IsLoadingClasses = false;
        }
    }

    [RelayCommand]
    private Task ViewSchedule()
        => Shell.Current.GoToAsync("//planning");

    [RelayCommand]
    private Task ViewPlans()
        => Shell.Current.GoToAsync("//account");

    /// <summary>
    /// Loads the subscription plans the same way the registration page does: show the
    /// SQLite cache instantly, then refresh from the API and update the cache.
    /// </summary>
    public async Task LoadSubscriptionPlansAsync()
    {
        IsLoadingPlans = true;
        try
        {
            // Fast path: fill from the cache so the section appears instantly.
            var cached = await _localDb.GetSubscriptionPlansAsync();
            if (cached.Count > 0)
            {
                ShowPlans(cached);
                IsLoadingPlans = false;
            }

            // Refresh from the API and update the cache for next time.
            var dto = await _subscriptionPlanManager.GetEntitiesAsync("SubscriptionPlan/plans") ?? [];
            var entities = dto.Select(CachedSubscriptionPlan.FromDto).ToList();
            ShowPlans(entities);
            await _localDb.SaveSubscriptionPlansAsync(entities);
        }
        catch
        {
            // Offline or API error: keep whatever the cache already gave us.
        }
        finally
        {
            IsLoadingPlans = false;
        }
    }

    private void ShowPlans(IEnumerable<CachedSubscriptionPlan> plans) =>
        Plans = [.. plans.Select(ToPlanInfo)];

    private static SubscriptionPlanInfo ToPlanInfo(CachedSubscriptionPlan plan) => new()
    {
        Name = plan.Name,
        Price = FormatPrice(plan.Price, plan.Currency),
        Description = BillingDescription(plan.DurationInMonths, plan.WeeklyCreditAmount),
        // Highlight the richer (effectively unlimited) plans, like the old mock did.
        Featured = plan.WeeklyCreditAmount >= 999
    };

    private static string FormatPrice(decimal price, string currency)
    {
        string symbol = currency?.ToUpperInvariant() switch
        {
            "EUR" => "€",
            "USD" => "$",
            "GBP" => "£",
            _ => currency is { Length: > 0 } ? currency.ToUpperInvariant() + " " : string.Empty
        };
        return $"{symbol}{price.ToString("0.##", CultureInfo.InvariantCulture)}";
    }

    private static string BillingDescription(int durationInMonths, int weeklyCreditAmount)
    {
        string billing = durationInMonths switch
        {
            1 => "maandelijks gefactureerd",
            12 => "jaarlijks gefactureerd",
            _ => $"per {durationInMonths} maanden gefactureerd"
        };
        string credits = weeklyCreditAmount >= 999
            ? "Onbeperkt trainen"
            : $"{weeklyCreditAmount} credits per week";
        return $"{credits}, {billing}";
    }
}
