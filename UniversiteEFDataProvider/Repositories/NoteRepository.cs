using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

//  ANCIEN CODE (hérite pas de Repository<Note>, duplication)
/*
public class NoteRepository(UniversiteDbContext context) : INoteRepository
{
    protected readonly UniversiteDbContext Context = context;

    public async Task<Note?> GetByIdAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.FindAsync(etudiantId, ueId);
    }

    public async Task<IEnumerable<Note>> GetAllAsync()
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.ToListAsync();
    }

    public async Task AddAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(note);
        Context.Notes.Add(note);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Note note)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        ArgumentNullException.ThrowIfNull(note);
        Context.Notes.Update(note);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        var note = await GetByIdAsync(etudiantId, ueId);
        if (note != null)
        {
            Context.Notes.Remove(note);
            await Context.SaveChangesAsync();
        }
    }

    public async Task<List<Note>> FindByConditionAsync(Expression<Func<Note, bool>> condition)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.Where(condition).ToListAsync();
    }

    public async Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .Where(n => n.EtudiantId == etudiantId)
            .ToListAsync();
    }

    public async Task<List<Note>> GetNotesByUeAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .Where(n => n.UeId == ueId)
            .ToListAsync();
    }
}
*/

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note?> GetByIdAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.FindAsync(etudiantId, ueId);
    }

    public async Task DeleteAsync(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        var note = await GetByIdAsync(etudiantId, ueId);
        if (note != null)
        {
            Context.Notes.Remove(note);
            await Context.SaveChangesAsync();
        }
    }

    // Méthodes de recherche spécifiques
    public async Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .Where(n => n.EtudiantId == etudiantId)
            .ToListAsync();
    }

    public async Task<List<Note>> GetNotesByUeAsync(long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes
            .Where(n => n.UeId == ueId)
            .ToListAsync();
    }
}