// csharp
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters
{
    public interface IUeRepository
    {
        Task<Ue?> GetByIdAsync(Guid id);
        Task<IEnumerable<Ue>> GetAllAsync();
        Task AddAsync(Ue ue);
        Task UpdateAsync(Ue ue);
        Task DeleteAsync(Guid id);
        Task<List<Ue>> FindByConditionAsync(Expression<Func<Ue, bool>> condition);
    }
}

