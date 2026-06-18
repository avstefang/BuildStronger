using Shared = Brand.Theme.BrandColors;

namespace MApp.Resources.Styles;

/// <summary>
/// Bridges the shared <see cref="Brand.Theme.BrandColors"/> hex strings into
/// MAUI <see cref="Color"/> instances so they can be referenced from XAML via
/// <c>x:Static</c> (see Colors.xaml). This keeps the brand palette defined in
/// exactly one place (the Brand project) while staying idiomatic in XAML.
/// </summary>
public static class ThemeColors
{
    public static readonly Color Primary = Color.FromArgb(Shared.PrimaryRed);
    public static readonly Color PrimaryDark = Color.FromArgb(Shared.PrimaryDark);
    public static readonly Color PrimaryLight = Color.FromArgb(Shared.PrimaryLight);
    public static readonly Color OnPrimary = Color.FromArgb(Shared.OnPrimary);

    public static readonly Color Secondary = Color.FromArgb(Shared.Secondary);

    public static readonly Color Background = Color.FromArgb(Shared.Background);
    public static readonly Color Surface = Color.FromArgb(Shared.Surface);
    public static readonly Color DarkBackground = Color.FromArgb(Shared.DarkBackground);
    public static readonly Color DarkSurface = Color.FromArgb(Shared.DarkSurface);

    public static readonly Color Success = Color.FromArgb(Shared.Success);
    public static readonly Color Warning = Color.FromArgb(Shared.Warning);
    public static readonly Color Error = Color.FromArgb(Shared.Error);
    public static readonly Color Info = Color.FromArgb(Shared.Info);
}
