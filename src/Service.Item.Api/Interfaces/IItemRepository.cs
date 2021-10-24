using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Item.Api.Contracts.Request;
using Service.Item.Api.Infrastructure.Entities;

namespace Service.Item.Api.Interfaces
{
    public interface IItemRepository
    {
        Task<Guid?> InsertAsync(Service.Item.Api.Infrastructure.Entities.Item entity);
        Task<int> InsertsAsync(Service.Item.Api.Infrastructure.Entities.Item[] entities);
        Task<IEnumerable<Infrastructure.Entities.Item>> QueryMultipleAsync(QueryItemRequest queryItemRequest);
        Task<Service.Item.Api.Infrastructure.Entities.Item> QueryOneAsync(Service.Item.Api.Infrastructure.Entities.Item query);
        Task<IEnumerable<Service.Item.Api.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(int[] itemIds);
        Task<int> UpdateAsync(Service.Item.Api.Infrastructure.Entities.Item entity);
    }
}
