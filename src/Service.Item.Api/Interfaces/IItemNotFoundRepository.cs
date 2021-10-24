using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Item.Api.Infrastructure.Entities;

namespace Service.Item.Api.Interfaces
{
    public interface IItemNotFoundRepository
    {
        Task<Guid?> InsertAsync(ItemNotFound entity);
        Task<IEnumerable<ItemNotFound>> QueryMultipleAsync(ItemNotFound query);
    }
}
