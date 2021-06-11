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
    public class ItemRepository : CoreRepository<Item>, IItemRepository
    {
        private const string SQLQUERY_SELECT_SINGLE_BY_ID = "SELECT TOP (1) * FROM Item WHERE ItemId = @ItemId;";

        private const string SQLQUERY_INSERT = "INSERT INTO Item (Id, ItemId, NameFrFr, NameEnUs, NameEnGb, Quality, ItemClass, ItemSubClass, InventoryType, Value, CreateAt, CreateBy) " +
                                                "VALUES (@Id, @ItemId, @NameFrFr, @NameEnUs, @NameEnGb, @Quality, @ItemClass, @ItemSubClass, @InventoryType, @Value, @CreateAt, @CreateBy);";

        private const string SQLQUERY_UPDATE = "UPDATE Item SET ItemId = @ItemId, NameFrFr = @NameFrFr, NameEnUs = @NameEnUs, NameEnGb = @NameEnGb, " +
                                        "Quality = @Quality, ItemClass = @ItemClass, ItemSubClass = @ItemSubClass, InventoryType = @InventoryType, " +
                                        "Value = @Value, UpdateAt = @UpdateAt, UpdateBy = @UpdateBy " +
                                        "WHERE ItemId = @ItemId;";

        public ItemRepository(IDatabaseConnectionFactory connectionFactory, ILogger<CoreRepository<Item>> logger) : base(connectionFactory, logger)
        {
        }

        public override Task<int> DeleteAsync(Item entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DeletesAsync(Item[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<Guid?> InsertAsync(Item entity)
        {
            return CoreInsertAsync(entity);
        }

        public override Task<int> InsertsAsync(Item[] entities)
        {
            return CoreInsertsAsync(SQLQUERY_INSERT, entities);
        }

        public override Task<IEnumerable<Item>> QueryMultipleAsync(Item query)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Item>> QueryMultipleByIdAsync(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public override Task<Item> QueryOneAsync(Item query)
        {
            var connection = connectionFactory.GetConnection();
            return connection.QueryFirstOrDefaultAsync<Item>(SQLQUERY_SELECT_SINGLE_BY_ID, query);
        }

        public override Task<Item> QueryOneByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(Item entity)
        {
            return CoreUpdateAsync(SQLQUERY_UPDATE, entity);
        }

        public override Task<int> UpdatesAsync(Item[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
