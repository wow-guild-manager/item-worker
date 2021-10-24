using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Item.Api.Infrastructure.Entities;

namespace Service.Item.Api.Interfaces
{
    public interface ISpellRepository
    {
        Task<Guid?> InsertAsync(Spell entity);
        Task<int> InsertsAsync(Spell[] entities);
        Task<IEnumerable<Spell>> QueryMultipleByIdAsync(int[] spellIds);
        Task<Spell> QueryOneAsync(Spell query);
        Task<int> UpdateAsync(Spell entity);
    }
}
