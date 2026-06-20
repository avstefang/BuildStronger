using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Application.Service;
using Domain.Entity;
using Domain.Repository;
using Domain.Value_object;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security;
using System.Security.Claims;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AthleteService athleteService, ITokenService tokenService, IConfiguration configuration) : ControllerBase
{
    private readonly AthleteService _athleteService = athleteService;
    private readonly ITokenService _tokenService = tokenService;

    /// <summary>
    /// Register a new athlete account
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterAthleteDto athleteDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            Athlete? athlete = await _athleteService.RegisterAthleteAsync(athleteDto);
            GetAthleteDto registeredAthlete = athlete?.ToDto() ?? throw new InvalidOperationException("Failed to create athlete");

            return Ok(registeredAthlete);
        }
        catch (AthleteAlreadyExistsException ex)
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Registration failed", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password to receive JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginAthleteDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Athlete? athlete = await _athleteService.LoginAthleteAsync(dto);

            if (athlete == null)
                return Unauthorized(new { error = "Invalid email or password" });

            var token = _tokenService.GenerateToken(athlete);

            return Ok(new LoginResponseDto(
                token,
                athlete.ToDto()
            ));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Login failed", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    [HttpPost("checktoken")]
    public IActionResult CheckToken([FromBody] string token)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool isValid = _tokenService.ValidateToken(token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Token validation failed", details = ex.InnerException?.Message ?? ex.Message });
        }
    }

    private string CreateToken(Athlete athlete)
    {
        List<Claim> claims = new();
        claims.Add(new Claim(ClaimTypes.Email, athlete.EmailAddress.ToString()));

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            configuration.GetValue<string>("Jwt:SecretKey") ?? throw new ArgumentNullException("No JWT secret key configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
