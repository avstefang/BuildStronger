using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Web.Services;

/// <summary>
/// Generic typed wrapper around the API — the Web counterpart of the MAUI app's EntityManager.
/// It attaches the staff JWT to every request and handles JSON (de)serialization, so pages just
/// call by URL and type, e.g. <c>await Api.GetListAsync&lt;GetWorkoutDto&gt;("api/workout")</c>.
/// </summary>
public class ApiClient(HttpClient http, TokenStore tokenStore)
{
    public async Task<List<T>> GetListAsync<T>(string url)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Get, url);
        if (!response.IsSuccessStatusCode)
            return [];

        return await response.Content.ReadFromJsonAsync<List<T>>() ?? [];
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Get, url);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<T>() : default;
    }

    /// <summary>POST returning the deserialized response (e.g. the login token).</summary>
    public async Task<TResponse?> PostAsync<TResponse>(string url, object body)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Post, url, body);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<TResponse>() : default;
    }

    /// <summary>POST returning only success/failure (e.g. create).</summary>
    public async Task<bool> PostAsync(string url, object body)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Post, url, body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync(string url, object? body = null)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Put, url, body);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string url)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Delete, url);
        return response.IsSuccessStatusCode;
    }

    /// <summary>POST that returns null on success, or the server's error text on failure (for surfacing details).</summary>
    public async Task<string?> PostReturningErrorAsync(string url, object body)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Post, url, body);
        return response.IsSuccessStatusCode ? null : await ReadErrorAsync(response);
    }

    /// <summary>PUT that returns null on success, or the server's error text on failure.</summary>
    public async Task<string?> PutReturningErrorAsync(string url, object body)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Put, url, body);
        return response.IsSuccessStatusCode ? null : await ReadErrorAsync(response);
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
    {
        try { return await response.Content.ReadAsStringAsync(); }
        catch { return $"HTTP {(int)response.StatusCode}"; }
    }

    /// <summary>
    /// Fetches an image (with the auth token) and returns it as a base64 data URL, so it can be bound
    /// directly to an &lt;img&gt;/MudCardMedia src — a plain image tag can't send the bearer token.
    /// </summary>
    public async Task<string?> GetImageSrcAsync(string url)
    {
        using HttpResponseMessage response = await SendAsync(HttpMethod.Get, url);
        if (!response.IsSuccessStatusCode)
            return null;

        byte[] bytes = await response.Content.ReadAsByteArrayAsync();
        string contentType = response.Content.Headers.ContentType?.ToString() ?? "image/jpeg";
        return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
    {
        HttpRequestMessage request = new(method, url);

        string? token = await tokenStore.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body is not null)
            request.Content = JsonContent.Create(body);

        return await http.SendAsync(request);
    }
}
