namespace Brand.Theme;

/// <summary>
/// Single source of truth for the BuildStronger brand palette.
/// <para>
/// Values are plain hex strings so they can be consumed by both UI stacks:
/// the Blazor (Web) <c>MudTheme</c> takes the strings directly, while the
/// MAUI (MApp) app converts them to <c>Microsoft.Maui.Graphics.Color</c> via its
/// own <c>ThemeColors</c> bridge. Change a colour here and both apps update.
/// </para>
/// </summary>
public static class BrandColors
{
    // Primary brand red
    public const string PrimaryRed = "#C62828";
    public const string PrimaryDark = "#8E0000";
    public const string PrimaryLight = "#FF5F52";
    public const string OnPrimary = "#FFFFFF";

    // Secondary / neutral
    public const string Secondary = "#212121";
    public const string OnSecondary = "#FFFFFF";

    // Surfaces (light theme)
    public const string Background = "#F5F5F5";
    public const string Surface = "#FFFFFF";

    // Surfaces (dark theme)
    public const string DarkBackground = "#17171A";
    public const string DarkSurface = "#222228";

    // Text
    public const string TextPrimary = "#212121";
    public const string TextSecondary = "#616161";

    // Status
    public const string Success = "#2E7D32";
    public const string Warning = "#F57F17";
    public const string Error = "#B71C1C";
    public const string Info = "#1565C0";
}
