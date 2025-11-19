using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository
{
    Task<Note?> GetByIdAsync(long etudiantId, long ueId);
    Task<IEnumerable<Note>> GetAllAsync();
    Task AddAsync(Note note);
    Task UpdateAsync(Note note);
    Task DeleteAsync(long etudiantId, long ueId);
    Task<List<Note>> FindByConditionAsync(Expression<Func<Note, bool>> condition);
    Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId);
    Task<List<Note>> GetNotesByUeAsync(long ueId);
}

