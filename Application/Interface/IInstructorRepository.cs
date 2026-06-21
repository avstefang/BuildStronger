using System;
using System.Collections.Generic;
using System.Text;
using Application.Dto;
using Domain.Entity;
using Domain.Value_object;

namespace Application.Interface;

public interface IInstructorRepository
{
    Task<Instructor> GetInstructorByIdAsync(Guid instructorId);
    Task<Instructor> GetInstructorByEmailAsync(EmailAddress emailAddress);
    Task<IEnumerable<Instructor>> GetAllInstructorsAsync();
    Task AddInstructorAsync(Instructor entity);
    Task UpdateInstructorAsync(UpdateAthleteDto entity);
    Task DeleteInstructorAsync(Guid instructorId);
}