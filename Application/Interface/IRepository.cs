using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interface;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}