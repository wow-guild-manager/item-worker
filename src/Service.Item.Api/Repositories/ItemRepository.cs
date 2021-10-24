using Dapper;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Item.Api.Contracts.Request;
using Service.Item.Api.Infrastructure.Entities;
using Service.Item.Api.Interfaces;

namespace Service.Item.Api.Repositories
{
    public class ItemRepository : CoreRepository<Service.Item.Api.Infrastructure.Entities.Item>, IItemRepository
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

        public override Task<Guid?> InsertAsync(Service.Item.Api.Infrastructure.Entities.Item entity)
        {
            return CoreInsertAsync(entity);
        }

        public override Task<int> InsertsAsync(Service.Item.Api.Infrastructure.Entities.Item[] entities)
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
                return await multi.ReadAsync<Service.Item.Api.Infrastructure.Entities.Item>();
            }
        }

        public override Task<IEnumerable<Service.Item.Api.Infrastructure.Entities.Item>> QueryMultipleAsync(Infrastructure.Entities.Item query)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<Service.Item.Api.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public override Task<Service.Item.Api.Infrastructure.Entities.Item> QueryOneAsync(Service.Item.Api.Infrastructure.Entities.Item query)
        {
            var connection = connectionFactory.GetConnection();
            return connection.QueryFirstOrDefaultAsync<Service.Item.Api.Infrastructure.Entities.Item>(SQLQUERY_SELECT_SINGLE_BY_ID, query);
        }

        public async Task<IEnumerable<Service.Item.Api.Infrastructure.Entities.Item>> QueryMultipleByIdAsync(int[] itemIds)
        {
            var connection = connectionFactory.GetConnection();

            using (var multi = connection.QueryMultiple(SQLQUERY_SELECT_BY_IDS, new { ItemIds = itemIds }))
            {
                return await multi.ReadAsync<Service.Item.Api.Infrastructure.Entities.Item>();
            }
        }

        public override Task<Service.Item.Api.Infrastructure.Entities.Item> QueryOneByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(Service.Item.Api.Infrastructure.Entities.Item entity)
        {
            return CoreUpdateAsync(SQLQUERY_UPDATE, entity);
        }

        public override Task<int> UpdatesAsync(Service.Item.Api.Infrastructure.Entities.Item[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
