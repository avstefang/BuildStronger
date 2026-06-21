using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http.Headers;

namespace MApp.Services;

public class EntityManager<T, Request>(IConfiguration config) where T : class
{
    private readonly string connectionString = config["Api:Endpoint"] ??
        throw new Exception("API base URL not configured. Please set 'Api:Endpoint' in appsettings.json or environment variables.");

    // The API emits camelCase JSON while the DTOs are PascalCase; without this the
    // properties deserialize to their defaults (null), e.g. a null login token.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<IEnumerable<T>?> GetEntitiesAsync(string endpoint, string? token = null)
    {
        using var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await httpClient.GetAsync($"{connectionString}/{endpoint}");

        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<T>>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> GetEntityAsync(string endpoint, string? token = null)
    {
        using var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await httpClient.GetAsync($"{connectionString}/{endpoint}");

        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> PostAsync(string endpoint, Request model)
    {
        using var httpClient = new HttpClient();
        var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{connectionString}/{endpoint}", jsonContent);
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> GetEntityByIdAsync(string endpoint, Request model, string id)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{connectionString}/{endpoint}/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> GetEntityByEmailOrUsernameAsync(string endpoint, Request model, string emailOrUsername)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{connectionString}/{endpoint}?emailOrUsername={emailOrUsername}");
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> CreateEntityAsync(string endpoint, Request model, string? token = null)
    {
        using var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(token))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{connectionString}/{endpoint}", jsonContent);
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> UpdateEntityAsync(string endpoint, Request? model, string? token = null, bool requestResponse = true)
    {
        using var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        HttpResponseMessage response;
        if (model == null)
        {
            StringContent jsonContent = new(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            response = await httpClient.PutAsync($"{connectionString}/{endpoint}", jsonContent);
        }
        else
        {
            response = await httpClient.PutAsync($"{connectionString}/{endpoint}", null);
        }

        if (response != null && response.IsSuccessStatusCode && requestResponse)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.OK))
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<bool> DeleteEntityAsync(string endpoint, string id, string? token = null)
    {
        using var httpClient = new HttpClient();
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await httpClient.DeleteAsync($"{connectionString}/{endpoint}/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response != null && (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.NotFound))
            return false;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }
}