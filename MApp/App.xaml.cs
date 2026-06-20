using MApp.Services;

namespace MApp
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        private readonly PresenceService _presence;

        public App(PresenceService presence)
        {
            InitializeComponent();
            _presence = presence;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Window window = new(new AppShell());

            // Drive the automatic check-in from the window lifecycle so it runs regardless of which
            // page is open. Resuming also fires an immediate check, so arriving at the gym doesn't
            // wait up to a minute for the next tick.
            _presence.Start();
            window.Resumed += (_, _) =>
            {
                _presence.Start();
                _ = _presence.TryAutoCheckInAsync();
            };
            window.Stopped += (_, _) => _presence.Stop();

            return window;
        }
    }
}
