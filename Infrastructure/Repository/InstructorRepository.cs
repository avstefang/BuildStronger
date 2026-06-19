using Application.Dto;
using Application.Interface;
using Domain.Entity;
using Domain.Value_object;
using Infrastructure.Context_model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class InstructorRepository(AthleteDbContext dbContext) : IInstructorRepository
{
    private IQueryable<Athlete> InstructorsQuery() =>
        dbContext.Set<Athlete>()
            .Include(a => a.Subscriptions).ThenInclude(s => s.SubscriptionPlan)
            .Where(a => a.Role == Domain.Enum.Role.Instructor);

    public async Task<Instructor> GetInstructorByIdAsync(Guid instructorId)
    {
        Athlete athlete = await InstructorsQuery().FirstOrDefaultAsync(a => a.Id == instructorId)
            ?? throw new InvalidOperationException($"Instructor with ID {instructorId} not found.");
        return new Instructor(athlete);
    }

    public async Task<Instructor> GetInstructorByEmailAsync(EmailAddress emailAddress)
    {
        Athlete athlete = await InstructorsQuery().FirstOrDefaultAsync(a => a.EmailAddress.Address == emailAddress.Address)
            ?? throw new InvalidOperationException($"Instructor with email '{emailAddress.Address}' not found.");
        return new Instructor(athlete);
    }

    public async Task<IEnumerable<Instructor>> GetAllInstructorsAsync()
    {
        var athletes = await InstructorsQuery().ToListAsync();
        return athletes.Select(a => new Instructor(a));
    }

    public async Task AddInstructorAsync(Instructor entity)
    {
        // Instructor is an Athlete with the Instructor role — promote the athlete
        entity.Athlete.PromoteToInstructor();
        dbContext.Set<Athlete>().Update(entity.Athlete);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateInstructorAsync(UpdateAthleteDto dto)
    {
        Athlete athlete = await dbContext.Set<Athlete>().FindAsync(dto.AthleteId)
            ?? throw new InvalidOperationException($"Instructor with ID {dto.AthleteId} not found.");

        EmailAddress emailAddress = new(dto.EmailAddress);
        athlete.ChangeEmailAddress(emailAddress);

        FullName fullName = new(dto.FirstName, dto.LastName);
        athlete.ChangeFullName(fullName);

        athlete.SetUsername(dto.Username);
        dbContext.Set<Athlete>().Update(athlete);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteInstructorAsync(Guid instructorId)
    {
        Athlete athlete = await dbContext.Set<Athlete>().FindAsync(instructorId)
            ?? throw new InvalidOperationException($"Instructor with ID {instructorId} not found.");
        athlete.DemoteToUser();
        dbContext.Set<Athlete>().Update(athlete);
        await dbContext.SaveChangesAsync();
    }
}
