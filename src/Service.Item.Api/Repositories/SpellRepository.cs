using Dapper;
using Infrastructure.Core.Interfaces;
using Infrastructure.Core.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.Item.Api.Infrastructure.Entities;
using Service.Item.Api.Interfaces;

namespace Service.Item.Api.Repositories
{
    public class SpellRepository : CoreRepository<Spell>, ISpellRepository
    {

        private const string SQLQUERY_SELECT_SINGLE_BY_ID = "SELECT TOP (1) * FROM Spell WHERE SpellId = @SpellId;";

        private const string SQLQUERY_INSERT = "INSERT INTO Spell (Id, SpellId, NameFrFr, NameEnUs, NameEnGb, DescriptionFrFr, DescriptionEnUs, DescriptionEnGb, Value, CreateAt, CreateBy) " +
                                                "VALUES (@Id, @SpellId, @NameFrFr, @NameEnUs, @NameEnGb, @DescriptionFrFr, @DescriptionEnUs, @DescriptionEnGb, @Value, @CreateAt, @CreateBy);";

        private const string SQLQUERY_UPDATE = "UPDATE Spell SET SpellId = @SpellId, NameFrFr = @NameFrFr, NameEnUs = @NameEnUs, NameEnGb = @NameEnGb, " +
                                        "DescriptionFrFr = @DescriptionFrFr, DescriptionEnUs = @DescriptionEnUs, DescriptionEnGb = @DescriptionEnGb, " +
                                        "Value = @Value, UpdateAt = @UpdateAt, UpdateBy = @UpdateBy " +
                                        "WHERE SpellId = @SpellId;";

        private const string SQLQUERY_SELECT_BY_IDS = "SELECT * FROM Spell WHERE SpellId IN @SpellIds;";

        public SpellRepository(IDatabaseConnectionFactory connectionFactory, ILogger<CoreRepository<Spell>> logger) : base(connectionFactory, logger)
        {
        }

        public override Task<int> DeleteAsync(Spell entity)
        {
            throw new NotImplementedException();
        }

        public override Task<int> DeletesAsync(Spell[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<Guid?> InsertAsync(Spell entity)
        {
            return CoreInsertAsync(entity);
        }

        public override Task<int> InsertsAsync(Spell[] entities)
        {
            return CoreInsertsAsync(SQLQUERY_INSERT, entities);
        }

        public async Task<IEnumerable<Spell>> QueryMultipleByIdAsync(int[] spellIds)
        {
            var connection = connectionFactory.GetConnection();

            using (var multi = connection.QueryMultiple(SQLQUERY_SELECT_BY_IDS, new { SpellIds = spellIds }))
            {
                return await multi.ReadAsync<Spell>();
            }
        }

        public override Task<IEnumerable<Spell>> QueryMultipleAsync(Spell query)
        {
            throw new NotImplementedException();

        }

        public override Task<IEnumerable<Spell>> QueryMultipleByIdAsync(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public override Task<Spell> QueryOneAsync(Spell query)
        {
            var connection = connectionFactory.GetConnection();
            return connection.QueryFirstOrDefaultAsync<Spell>(SQLQUERY_SELECT_SINGLE_BY_ID, query);
        }

        public override Task<Spell> QueryOneByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public override Task<int> UpdateAsync(Spell entity)
        {
            return CoreUpdateAsync(SQLQUERY_UPDATE, entity);
        }

        public override Task<int> UpdatesAsync(Spell[] entities)
        {
            throw new NotImplementedException();
        }
    }
}
