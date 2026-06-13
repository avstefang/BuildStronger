using Application.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repository;

public abstract class Repository<T, Id> where T : class
{
    protected DbContext DbContext;

    protected Repository(DbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected async Task AddAsync(T entity)
    {
        await DbContext.Set<T>().AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }

    protected async Task DeleteAsync(Id id)
    {
        T entity = await DbContext.Set<T>().FindAsync(id) ?? throw new Exception("Entity not found");
        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync();
    }

    protected async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbContext.Set<T>().ToListAsync();
    }

    protected async Task<T?> GetByIdAsync(Id id)
    {
        return await DbContext.Set<T>().FindAsync(id);
    }

    private bool EntityExists(T entity)
    {
        var key = DbContext.Model.FindEntityType(typeof(T))!
            .FindPrimaryKey()!
            .Properties
            .Select(p => p.PropertyInfo!.GetValue(entity))
            .ToArray();
        return DbContext.Set<T>().Find(key) != null;
    }

    protected async Task UpdateAsync(T entity)
    {
        if (!EntityExists(entity))
        {
            throw new Exception("Entity not found");
        }

        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync();
    }
}