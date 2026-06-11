using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IRepository<T, Id> where T : class
{
    Task<T?> GetByIdAsync(Id id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Id id);
}