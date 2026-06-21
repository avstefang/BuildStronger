using Application.Service;
using Domain.Entity;
using Domain.Value_object;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PhotoController(IWebHostEnvironment environment, AthleteService athleteService) : ControllerBase
{
    // Stored outside wwwroot (which doesn't exist) under the content root so the folder is always available.
    private string UploadsFolder => Path.Combine(environment.ContentRootPath, "uploads");

    [Authorize]
    [HttpGet("{filename}")]
    public IActionResult GetPhoto(string filename)
    {
        // GetFileName strips any directory parts, blocking path-traversal (e.g. "../appsettings.json").
        string safeName = Path.GetFileName(filename);
        string filePath = Path.Combine(UploadsFolder, safeName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, GetContentType(safeName));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadPhoto(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        Athlete? athlete;
        try
        {
            EmailAddress email = new(User.FindFirstValue(ClaimTypes.Email) ?? throw new InvalidOperationException("User email not found"));
            athlete = await athleteService.GetAthleteByEmailAsync(email);
        }
        catch (Exception)
        {
            return Unauthorized();
        }

        if (athlete == null)
            return NotFound("Athlete not found.");

        Directory.CreateDirectory(UploadsFolder);

        // One photo per athlete. Remove any previous file so they don't accumulate, and use a unique
        // name each time so the stored path changes (the app caches by filename, so this busts the cache).
        foreach (string old in Directory.EnumerateFiles(UploadsFolder, $"{athlete.Id}_*"))
            System.IO.File.Delete(old);

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension))
            extension = ".jpg";

        string storedName = $"{athlete.Id}_{DateTime.UtcNow.Ticks}{extension}";
        string filePath = Path.Combine(UploadsFolder, storedName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        await athleteService.UploadPhotoAsync(athlete.EmailAddress, storedName);
        return Ok(new { filename = storedName });
    }

    private static string GetContentType(string filename) => Path.GetExtension(filename).ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "image/jpeg"
    };
}
