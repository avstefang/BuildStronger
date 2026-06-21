using Microsoft.JSInterop;

namespace Web.Services;

/// <summary>
/// Persists the staff JWT in the browser's localStorage so it survives a page refresh.
/// Login is restricted to staff accounts, so a stored token means "signed in as staff".
/// </summary>
public class TokenStore(IJSRuntime js)
{
    private const string Key = "bs_staff_token";

    public ValueTask SetAsync(string token) => js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask<string?> GetAsync() => js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask ClearAsync() => js.InvokeVoidAsync("localStorage.removeItem", Key);
}
