using Dapper;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Worker.Contracts.Request;
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

        private const string SQLQUERY_SEARCH = "SELECT * FROM Item WHERE ItemId = @Search " +
                                                "OR NameFrFr LIKE CONCAT('%', @Search, '%') " +
                                                "OR NameEnUs LIKE CONCAT('%', @Search, '%') " +
                                                "OR NameEnGb LIKE CONCAT('%', @Search, '%')";

        private const string SQLQUERY_SEARCH_WITHOUTID = "SELECT * FROM Item WHERE " +
                                        "NameFrFr LIKE CONCAT('%', @Search, '%') " +
                                        "OR NameEnUs LIKE CONCAT('%', @Search, '%') " +
                                        "OR NameEnGb LIKE CONCAT('%', @Search, '%')";

        public ItemRepository(IDatabaseConnectionFactory connectionFactory, ILogger<CoreRepository<Infrastructure.Entities.Item>> logger) 
            : base(connectionFactory, logger)
        {
        }

        public override Task<int> DeleteAsync(Infrastructure.Entities.Item entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DeletesAsync(Infrastructure.Entities.Item[] entities)
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

        public async Task<IEnumerable<Infrastructure.Entities.Item>> QueryMultipleAsync(QueryItemRequest queryItemRequest)
        {
            var connection = connectionFactory.GetConnection();
            var query = int.TryParse(queryItemRequest.Search, out int _) ? SQLQUERY_SEARCH : SQLQUERY_SEARCH_WITHOUTID;

            if (!string.IsNullOrEmpty(queryItemRequest.Quality))
            {
                query += " AND Quality = @Quality";
            }

            using (var multi = connection.QueryMultiple(query, queryItemRequest))
            {
                return await multi.ReadAsync<Worker.Infrastructure.Entities.Item>();
            }
        }

        public override Task<IEnumerable<Worker.Infrastructure.Entities.Item>> QueryMultipleAsync(Infrastructure.Entities.Item query)
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

            using (var multi = connection.QueryMultiple(SQLQUERY_SELECT_BY_IDS, new { ItemIds = itemIds }))
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
