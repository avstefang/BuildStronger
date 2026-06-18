using Application.Dto;
using Application.Interface;
using Application.Mapping;
using Application.Template;
using Domain.Entity;
using Domain.Exception;
using Domain.Value_object;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Application.Service;

public class AthleteService(IAthleteRepository athleteRepository, IPasswordCrypt passwordCrypt, IEmailSender? emailSender, ISubscriptionRepository subscriptionRepository)
{
    private readonly IAthleteRepository _athleteRepository = athleteRepository;
    private readonly IPasswordCrypt _passwordCrypt = passwordCrypt;
    private readonly IEmailSender? _emailSender = emailSender;

    public async Task<Athlete?> RegisterAthleteAsync(RegisterAthleteDto athleteDto)
    {
        string passwordHash = _passwordCrypt.HashPassword(athleteDto.Password);
        EmailAddress email = new(athleteDto.Email);
        FullName fullName = new(athleteDto.FirstName, athleteDto.LastName);

        string username = email.Address.Split("@")[0];
        int count = 0;
        while (await _athleteRepository.GetAthleteByUsernameAsync(username) != null)
        {
            count++;
            username = $"{email.Address.Split("@")[0]}{count}";
        }

        Athlete athlete = new(email, fullName, passwordHash);
        athlete.SetUsername(username);

        // Save the athlete to the repository
        await _athleteRepository.AddAthleteAsync(athlete);

        // Send a welcome email if email sender is provided
        if (_emailSender != null)
        {
            EmailContentDto template = EmailTemplate.WelcomeEmail(athlete.FullName);
            await _emailSender.SendEmailAsync(athlete.EmailAddress, template.Subject, template.Body);
        }

        return athlete;
    }

    public async Task<Athlete?> LoginAthleteAsync(LoginAthleteDto dto)
    {
        EmailAddress? email = null;
        string? username = null;
        try { email = new(dto.UsernameOrEmail); }
        catch { username = dto.UsernameOrEmail; }

        Athlete? athlete = email != null ? await _athleteRepository.GetAthleteByEmailAsync(email) : 
            await _athleteRepository.GetAthleteByUsernameAsync(username!);
        if (athlete == null) return null;

        bool verifyPassword = _passwordCrypt.VerifyPassword(dto.Password, athlete.Password);
        return verifyPassword ? athlete : null;
    }

    public async Task<Athlete?> UpdateAthlete(UpdateAthleteDto updateAthleteDto)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(updateAthleteDto.EmailAddress);
        if (athlete == null) return null;

        await _athleteRepository.UpdateAthleteAsync(athlete);
        return athlete;
    }

    public async Task<GetAthleteDto?> GetAthleteByEmail(EmailAddress emailAddress)
    {
        try
        {
            return (await _athleteRepository.GetAthleteByEmailAsync(emailAddress))?.ToDto();
        }
        catch (AthleteNotFoundException)
        {
            return null;
        }
    }

    public async Task UpdateAthletePasswordAsync(UpdatePasswordDto updatePasswordDto)
    {
        if (updatePasswordDto.CurrentPassword == updatePasswordDto.NewPassword)
            throw new ArgumentException("New password cannot be the same as the current password");

        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(new EmailAddress(updatePasswordDto.Email));
        if (athlete == null)
            throw new AthleteNotFoundException($"Athlete with email address {updatePasswordDto.Email} not found");

        if (!_passwordCrypt.VerifyPassword(updatePasswordDto.CurrentPassword, athlete.Password))
            throw new ArgumentException("Current password is incorrect");

        string newPasswordHash = _passwordCrypt.HashPassword(updatePasswordDto.NewPassword);
        athlete.SetPassword(newPasswordHash);
        await _athleteRepository.UpdateAthleteAsync(athlete);
    }

    public async Task UpdateUsernameAsync(UpdateUsernameDto updateUsernameDto)
    {
        if (await _athleteRepository.IsAthleteUsernameTakenAsync(updateUsernameDto.Username))
            throw new ArgumentException($"Username {updateUsernameDto.Username} is already taken");

        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(new EmailAddress(updateUsernameDto.Email));
        if (athlete == null)
            throw new AthleteNotFoundException($"Athlete with email address {updateUsernameDto.Email} not found");
        athlete.SetUsername(updateUsernameDto.Username);
        await _athleteRepository.UpdateAthleteAsync(athlete);
    }

    public async Task<IEnumerable<GetSubscriptionDto>?> GetAthleteSubscriptionsAsync(EmailAddress email)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(email);
        if (athlete == null)
            throw new AthleteNotFoundException($"Athlete with email address {email.Address} not found");
        IEnumerable<Subscription>? subscriptions = await subscriptionRepository.GetSubscriptionsByAthleteIdAsync(athlete.Id);
        return subscriptions?.Select(subscription => subscription.ToDto());
    }

    public async Task<IEnumerable<GetAthleteDto>> GetAllAthletesAsync()
    {
        return (await _athleteRepository.GetAllAthletesAsync()).Select(athlete => athlete.ToDto());
    }

    public async Task DeleteAthleteByEmailAsync(EmailAddress email)
    {
        await _athleteRepository.DeleteAthleteByEmailAsync(email);
    }

    public async Task PromoteToInstructorAsync(EmailAddress email)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(email);
        if (athlete == null)
            throw new AthleteNotFoundException($"Athlete with email address {email.Address} not found");
        athlete.PromoteToInstructor();
        await _athleteRepository.UpdateAthleteAsync(athlete);
    }

    public async Task DemoteToUserAsync(EmailAddress email)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(email) ??
            throw new AthleteNotFoundException($"Athlete with email address {email.Address} not found");

        athlete.DemoteToUser();
        await _athleteRepository.UpdateAthleteAsync(athlete);
    }

    public async Task UploadProfilePicture(EmailAddress email, string photoPath)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(email) ??
            throw new AthleteNotFoundException($"Athlete with email address {email.Address} not found");
        athlete.PhotoPath = new PhotoPath(photoPath);
        await _athleteRepository.UpdateAthleteAsync(athlete);
    }

    public async Task<Subscription?> GetActiveSubscriptionAsync(EmailAddress email)
    {
        Athlete? athlete = await _athleteRepository.GetAthleteByEmailAsync(email) ??
            throw new AthleteNotFoundException($"Athlete with email address {email.Address} not found");
        return athlete.GetActiveSubscription();
    }
}