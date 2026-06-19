using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MApp.Services;

public class EntityModificationManager<T, Request>(IConfiguration config) where T : class
{
    private readonly string connectionString = config["Api:Endpoint"] ??
        throw new Exception("API base URL not configured. Please set 'Api:Endpoint' in appsettings.json or environment variables.");

    // The API emits camelCase JSON while the DTOs are PascalCase; without this the
    // properties deserialize to their defaults (null), e.g. a null login token.
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T?> CreateEntityAsync(string endpoint, Request model, T entity)
    {
        using var httpClient = new HttpClient();
        var jsonContent = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"{connectionString}/{endpoint}", jsonContent);
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<T?> UpdateEntityAsync(string endpoint, Request model, string id, T entity)
    {
        using var httpClient = new HttpClient();
        var jsonContent = new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8, "application/json");
        var response = await httpClient.PutAsync($"{connectionString}/{endpoint}/{id}", jsonContent);
        if (response != null && response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }

    public async Task<bool> DeleteEntityAsync(string endpoint, string id)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.DeleteAsync($"{connectionString}/{endpoint}/{id}");
        if (response != null && response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
            return default;

        throw new Exception($"Login failed. Status code: {response?.StatusCode.ToString() ?? "No Status Code"}");
    }
}