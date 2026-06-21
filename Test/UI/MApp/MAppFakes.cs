using MApp.Services;

namespace Test.UI.MApp;

/// <summary>No-op IAuthService for PageModel tests — avoids SecureStorage (MAUI-only).</summary>
internal sealed class FakeAuthService : IAuthService
{
    public Task<bool> IsAuthenticatedAsync() => Task.FromResult(false);
    public Task SaveTokenAsync(string token) => Task.CompletedTask;
    public Task<string?> GetTokenAsync() => Task.FromResult<string?>(null);
    public void Logout() { }
}
