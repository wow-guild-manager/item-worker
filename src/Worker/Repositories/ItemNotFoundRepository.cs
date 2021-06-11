using Dapper;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;
using Worker.Interfaces;

namespace Worker.Repositories
{
    public class ItemNotFoundRepository : CoreRepository<ItemNotFound>, IItemNotFoundRepository
    {
        private const string SQLQUERY_SELECT_USER_ALL = "SELECT * FROM ItemNotFound";

        public ItemNotFoundRepository(IDatabaseConnectionFactory connectionFactory, ILogger<CoreRepository<ItemNotFound>> logger) : base(connectionFactory, logger)
        {
        }

        public override Task<int> DeleteAsync(ItemNotFound entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DeletesAsync(ItemNotFound[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<Guid?> InsertAsync(ItemNotFound entity)
        {
            return CoreInsertAsync(entity);
        }

        public override Task<int> InsertsAsync(ItemNotFound[] entities)
        {
            throw new NotImplementedException();
        }

        public override async Task<IEnumerable<ItemNotFound>> QueryMultipleAsync(ItemNotFound query)
        {
            var connection = connectionFactory.GetConnection();

            using (var multi = connection.QueryMultiple(SQLQUERY_SELECT_USER_ALL, query))
            {
                return await multi.ReadAsync<Infrastructure.Entities.ItemNotFound>();
            }
        }

        public override Task<IEnumerable<ItemNotFound>> QueryMultipleByIdAsync(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemNotFound> QueryOneAsync(ItemNotFound query)
        {
            throw new NotImplementedException();
        }

        public override Task<ItemNotFound> QueryOneByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(ItemNotFound entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdatesAsync(ItemNotFound[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
