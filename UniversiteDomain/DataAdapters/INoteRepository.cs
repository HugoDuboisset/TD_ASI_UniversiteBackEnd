using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    Task<Note?> GetByIdAsync(long etudiantId, long ueId);
    Task DeleteAsync(long etudiantId, long ueId);

    Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId);
    Task<List<Note>> GetNotesByUeAsync(long ueId);
}

