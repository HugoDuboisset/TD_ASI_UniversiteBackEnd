// csharp
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters
{
    // ❌ ANCIEN CODE (problème critique : utilise Guid au lieu de long, et ne hérite pas de IRepository<Ue>)
    /*
    public interface IUeRepository
    {
        Task<Ue?> GetByIdAsync(Guid id);  // ❌ Guid au lieu de long
        Task<IEnumerable<Ue>> GetAllAsync();
        Task AddAsync(Ue ue);
        Task UpdateAsync(Ue ue);
        Task DeleteAsync(Guid id);  // ❌ Guid au lieu de long
        Task<List<Ue>> FindByConditionAsync(Expression<Func<Ue, bool>> condition);
    }
    */

    // ✅ NOUVEAU CODE : utilise long et hérite de IRepository<Ue> pour cohérence
    public interface IUeRepository : IRepository<Ue>
    {
        // Toutes les méthodes CRUD sont héritées de IRepository<Ue>
        // Pas besoin de les redéfinir ici
    }
}

