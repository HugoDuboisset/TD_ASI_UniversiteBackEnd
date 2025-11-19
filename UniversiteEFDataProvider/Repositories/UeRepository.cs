using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class UeRepository(UniversiteDbContext context) : IUeRepository
{
    protected readonly UniversiteDbContext Context = context;

    public async Task<Ue?> GetByIdAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues.FindAsync(id);
    }

    public async Task<IEnumerable<Ue>> GetAllAsync()
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues.ToListAsync();
    }

    public async Task AddAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(ue);
        
        Context.Ues.Add(ue);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        ArgumentNullException.ThrowIfNull(ue);
        
        Context.Ues.Update(ue);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        
        var ue = await GetByIdAsync(id);
        if (ue != null)
        {
            Context.Ues.Remove(ue);
            await Context.SaveChangesAsync();
        }
    }

    public async Task<List<Ue>> FindByConditionAsync(Expression<Func<Ue, bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(Context.Ues);
        return await Context.Ues.Where(condition).ToListAsync();
    }
}