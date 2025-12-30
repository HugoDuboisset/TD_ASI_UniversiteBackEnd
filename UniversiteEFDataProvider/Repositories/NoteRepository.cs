using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

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