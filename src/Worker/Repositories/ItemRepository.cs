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
    public class ItemRepository : CoreRepository<Worker.Infrastructure.Entities.Item>, IItemRepository
    {
        private const string SQLQUERY_SELECT_SINGLE_BY_ID = "SELECT TOP (1) * FROM Item WHERE ItemId = @ItemId;";

        private const string SQLQUERY_INSERT = "INSERT INTO Item (Id, ItemId, NameFrFr, NameEnUs, NameEnGb, Quality, ItemClass, ItemSubClass, InventoryType, Value, CreateAt, CreateBy) " +
                                                "VALUES (@Id, @ItemId, @NameFrFr, @NameEnUs, @NameEnGb, @Quality, @ItemClass, @ItemSubClass, @InventoryType, @Value, @CreateAt, @CreateBy);";

        private const string SQLQUERY_UPDATE = "UPDATE Item SET ItemId = @ItemId, NameFrFr = @NameFrFr, NameEnUs = @NameEnUs, NameEnGb = @NameEnGb, " +
                                        "Quality = @Quality, ItemClass = @ItemClass, ItemSubClass = @ItemSubClass, InventoryType = @InventoryType, " +
                                        "Value = @Value, UpdateAt = @UpdateAt, UpdateBy = @UpdateBy " +
                                        "WHERE ItemId = @ItemId;";

        private const string SQLQUERY_SELECT_BY_IDS = "SELECT * FROM Item WHERE ItemId IN @ItemIds;";

        public ItemRepository(IDatabaseConnectionFactory connectionFactory, ILogger<CoreRepository<Worker.Infrastructure.Entities.Item>> logger) : base(connectionFactory, logger)
        {
        }

        public override Task<int> DeleteAsync(Worker.Infrastructure.Entities.Item entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DeletesAsync(Worker.Infrastructure.Entities.Item[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<Guid?> InsertAsync(Worker.Infrastructure.Entities.Item entity)
        {
            return CoreInsertAsync(entity);
        }

        public override Task<int> InsertsAsync(Worker.Infrastructure.Entities.Item[] entities)
        {
            return CoreInsertsAsync(SQLQUERY_INSERT, entities);
        }

        public override Task<IEnumerable<Worker.Infrastructure.Entities.Item>> QueryMultipleAsync(Worker.Infrastructure.Entities.Item query)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Worker.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public override Task<Worker.Infrastructure.Entities.Item> QueryOneAsync(Worker.Infrastructure.Entities.Item query)
        {
            var connection = connectionFactory.GetConnection();
            return connection.QueryFirstOrDefaultAsync<Worker.Infrastructure.Entities.Item>(SQLQUERY_SELECT_SINGLE_BY_ID, query);
        }

        public async Task<IEnumerable<Worker.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(int[] itemIds)
        {
            var connection = connectionFactory.GetConnection();

            using (var multi = connection.QueryMultiple(SQLQUERY_SELECT_BY_IDS, itemIds))
            {
                return await multi.ReadAsync<Worker.Infrastructure.Entities.Item>();
            }
        }

        public override Task<Worker.Infrastructure.Entities.Item> QueryOneByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(Worker.Infrastructure.Entities.Item entity)
        {
            return CoreUpdateAsync(SQLQUERY_UPDATE, entity);
        }

        public override Task<int> UpdatesAsync(Worker.Infrastructure.Entities.Item[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
