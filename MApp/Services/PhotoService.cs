using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MApp.Services;

/// <summary>
/// Handles profile photos against the API. The generic <see cref="EntityManager{T,Request}"/>
/// only deals with JSON, so multipart upload and authenticated image download live here.
/// </summary>
public class PhotoService(IConfiguration config)
{
    private readonly string _baseUrl = config["Api:Endpoint"] ??
        throw new Exception("API base URL not configured. Please set 'Api:Endpoint' in appsettings.json.");

    /// <summary>Uploads an image and returns the server filename to store, or null on failure.</summary>
    public async Task<string?> UploadAsync(string token, Stream content, string fileName, string? contentType)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var form = new MultipartFormDataContent();
        var fileContent = new StreamContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "image/jpeg" : contentType);
        // "file" must match the IFormFile parameter name on PhotoController.UploadPhoto.
        form.Add(fileContent, "file", fileName);

        var response = await httpClient.PostAsync($"{_baseUrl}/Photo", form);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("filename", out var name) ? name.GetString() : null;
    }

    /// <summary>Downloads the photo (sending the bearer token) into the cache folder and returns the local path, or null.</summary>
    public async Task<string?> DownloadToCacheAsync(string token, string fileName)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetAsync($"{_baseUrl}/Photo/{fileName}");
        if (!response.IsSuccessStatusCode)
            return null;

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var localPath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(localPath, bytes);
        return localPath;
    }
}
