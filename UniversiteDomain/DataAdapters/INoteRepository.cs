using System.Linq.Expressions;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

//  ANCIEN CODE ( hérite pas de IRepository<Note>)
/*
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
*/

// ✅ NOUVEAU CODE : hérite de IRepository<Note> et garde uniquement les méthodes spécifiques
public interface INoteRepository : IRepository<Note>
{
    // Méthodes spécifiques pour Note avec clé composite (EtudiantId, UeId)
    Task<Note?> GetByIdAsync(long etudiantId, long ueId);
    Task DeleteAsync(long etudiantId, long ueId);

    // Méthodes de recherche spécifiques
    Task<List<Note>> GetNotesByEtudiantAsync(long etudiantId);
    Task<List<Note>> GetNotesByUeAsync(long ueId);
}

