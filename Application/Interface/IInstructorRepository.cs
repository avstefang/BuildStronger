using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entity;

namespace Application.Interface;

public interface IInstructorRepository : IRepository<Instructor>
{
    Task<Instructor> GetInstructorAsync(Instructor entity);
    Task<IEnumerable<Instructor>> GetAllInstructorsAsync();
    Task AddInstructorAsync(Instructor entity);
    Task UpdateInstructorAsync(Instructor entity);
    Task DeleteInstructorAsync(Instructor entity);
}
