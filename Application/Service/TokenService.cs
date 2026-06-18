using Application.Interface;
using Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Application.Service;

public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken(Athlete athlete)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey ?? throw new ArgumentNullException("No JWT secret key configured")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = [
            new(ClaimTypes.NameIdentifier, athlete.Id.ToString()),
            new(ClaimTypes.Email, athlete.EmailAddress.Address),
            new(ClaimTypes.Name, athlete.FullName.ToString()),
            new(ClaimTypes.Role, athlete.Role.ToString()),
            new(ClaimTypes.AuthorizationDecision, athlete.GetActiveSubscription() != null ? "true" : "false")
        ];

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = GetValidationParameters();

        SecurityToken validatedToken;
        IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
        return true;
    }

    private TokenValidationParameters GetValidationParameters()
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        return new TokenValidationParameters()
        {
            ValidateLifetime = true, // Because there is no expiration in the generated token
            ValidateAudience = true, // Because there is no audiance in the generated token
            ValidateIssuer = true,   // Because there is no issuer in the generated token
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new ArgumentNullException("No JWT secret key configured"))) // The same key as the one that generate the token
        };
    }
}

