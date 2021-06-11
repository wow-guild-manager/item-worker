using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;

namespace Worker.Interfaces
{
    public interface IItemNotFoundRepository
    {
        Task<Guid?> InsertAsync(ItemNotFound entity);
        Task<IEnumerable<ItemNotFound>> QueryMultipleAsync(ItemNotFound query);
    }
}
