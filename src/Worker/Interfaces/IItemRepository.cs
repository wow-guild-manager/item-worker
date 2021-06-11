using System;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;

namespace Worker.Interfaces
{
    public interface IItemRepository
    {
        Task<Guid?> InsertAsync(Item entity);
        Task<int> InsertsAsync(Item[] entities);
        Task<Item> QueryOneAsync(Item query);
        Task<int> UpdateAsync(Item entity);
    }
}
