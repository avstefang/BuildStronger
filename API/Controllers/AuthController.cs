using Application.Dto;
using Application.Interface;
using Application.Service;
using Domain.Entity;
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
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _athleteService.RegisterAthleteAsync(athleteDto);

            return Ok(new { message = "Athlete registered successfully", emailAddress = athleteDto.Email });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Registration failed", details = ex.Message });
        }
    }

    /// <summary>
    /// Login with email and password to receive JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var email = new EmailAddress(dto.Email);

            // Convert string to SecureString for password verification
            var securePassword = new SecureString();
            foreach (char c in dto.Password)
                securePassword.AppendChar(c);
            securePassword.MakeReadOnly();

            Athlete? athlete = await _athleteService.LoginAthleteAsync(email, securePassword);

            if (athlete == null)
                return Unauthorized(new { error = "Invalid email or password" });

            var token = _tokenService.GenerateToken(athlete);

            return Ok(new 
            { 
                message = "Login successful",
                token,
                athlete
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Login failed", details = ex.Message });
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
