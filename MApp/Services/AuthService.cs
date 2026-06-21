using System.Text;
using System.Text.Json;

namespace MApp.Services;

public interface IAuthService
{
    /// <summary>True if a stored token exists and has not expired.</summary>
    Task<bool> IsAuthenticatedAsync();
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    void Logout();
}

public class AuthService : IAuthService
{
    private const string TokenKey = "authToken";
    private LocalDbService dbService = new();

    public Task SaveTokenAsync(string token) => SecureStorage.SetAsync(TokenKey, token);

    public Task<string?> GetTokenAsync() => SecureStorage.GetAsync(TokenKey);

    public async void Logout()
    {
        SecureStorage.Remove(TokenKey);
        await dbService.ClearAthleteAsync();
        await dbService.ClearBookingsAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        string? token = await SecureStorage.GetAsync(TokenKey);
        if (string.IsNullOrWhiteSpace(token))
            return false;

        if (IsExpired(token))
        {
            // An expired token is as good as no token; clear it so we don't retry it.
            SecureStorage.Remove(TokenKey);
            return false;
        }

        return true;
    }

    // Reads the JWT 'exp' claim directly: a JWT is three base64url segments and the
    // middle one is JSON. This avoids pulling a JWT library into the client.
    private static bool IsExpired(string token)
    {
        try
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                return true;

            using JsonDocument payload = JsonDocument.Parse(Base64UrlDecode(parts[1]));
            if (!payload.RootElement.TryGetProperty("exp", out JsonElement expElement))
                return false; // no expiry claim → treat as non-expiring

            DateTimeOffset expiry = DateTimeOffset.FromUnixTimeSeconds(expElement.GetInt64());
            return expiry <= DateTimeOffset.UtcNow;
        }
        catch
        {
            // Malformed token → treat as expired/invalid.
            return true;
        }
    }

    private static string Base64UrlDecode(string input)
    {
        string base64 = input.Replace('-', '+').Replace('_', '/');
        base64 += (base64.Length % 4) switch
        {
            2 => "==",
            3 => "=",
            _ => string.Empty
        };
        return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
    }
}
