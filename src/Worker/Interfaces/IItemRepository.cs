using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;

namespace Worker.Interfaces
{
    public interface IItemRepository
    {
        Task<Guid?> InsertAsync(Worker.Infrastructure.Entities.Item entity);
        Task<int> InsertsAsync(Worker.Infrastructure.Entities.Item[] entities);
        Task<Worker.Infrastructure.Entities.Item> QueryOneAsync(Worker.Infrastructure.Entities.Item query);
        Task<IEnumerable<Worker.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(int[] itemIds);
        Task<int> UpdateAsync(Worker.Infrastructure.Entities.Item entity);
    }
}
