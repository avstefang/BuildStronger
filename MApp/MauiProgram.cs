using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using System.Reflection;

namespace MApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if WINDOWS
    				Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler.Mapper.AppendToMapping("KeyboardAccessibleCollectionView", (handler, view) =>
    				{
    					handler.PlatformView.SingleSelectionFollowsFocus = false;
    				});
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("MApp.appsettings.json");
            if (stream is not null)
            {
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
                builder.Configuration.AddConfiguration(config);
            }

            builder.Services.AddScoped(typeof(EntityManager<,>));

            builder.Services.AddSingleton<IAuthService, AuthService>();

            builder.Services.AddSingleton<PhotoService>();

            // Automatic geofence check-in (runs a once-a-minute presence check while the app is open)
            builder.Services.AddSingleton<PresenceService>();

            // On-device SQLite cache (subscription plans, etc.)
            builder.Services.AddSingleton<LocalDbService>();

            builder.Services.AddSingleton<ModalErrorHandler>();

            // Loading (startup page) — routes to home or login based on the stored token
            builder.Services.AddSingleton<LoadingPageModel>();
            builder.Services.AddTransient<LoadingPage>();

            // Login
            builder.Services.AddSingleton<LoginPageModel>();
            builder.Services.AddTransient<LoginPage>();

            // Pushed routes
            builder.Services.AddTransientWithShellRoute<RegisterPage, RegisterPageModel>("register");
            builder.Services.AddTransientWithShellRoute<PaymentPage, PaymentPageModel>("payment");
            builder.Services.AddTransientWithShellRoute<AddSubscriptionPage, AddSubscriptionPageModel>("addsubscription");
            builder.Services.AddTransientWithShellRoute<ChangeSubscriptionPage, ChangeSubscriptionPageModel>("changesubscription");
            builder.Services.AddTransientWithShellRoute<ChangeSubscriptionPage, ChangeSubscriptionPageModel>("changesubscription");
            builder.Services.AddTransientWithShellRoute<EditProfilePage, EditProfilePageModel>("editprofile");

            // Tab pages + their view models
            builder.Services.AddSingleton<HomePageModel>();
            builder.Services.AddSingleton<PlanningPageModel>();
            builder.Services.AddSingleton<BookingsPageModel>();
            builder.Services.AddSingleton<AccountPageModel>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<PlanningPage>();
            builder.Services.AddTransient<BookingsPage>();
            builder.Services.AddTransient<AccountPage>();

            // Detail page reached via Shell route navigation (carries the class id)
            builder.Services.AddTransientWithShellRoute<BookClassPage, BookClassPageModel>("bookclass");

            return builder.Build();
        }
    }
}
