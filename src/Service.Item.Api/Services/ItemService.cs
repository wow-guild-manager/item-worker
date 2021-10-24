using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WGM.Service.Item;
using Service.Item.Api.Interfaces;

namespace Service.Item.Api.Services
{
    public class ItemService : ItemGrpcService.ItemGrpcServiceBase
    {
        private readonly ILogger<ItemService> logger;
        private readonly IItemRepository itemRepository;
        private readonly ISpellRepository spellRepository;

        public ItemService(ILogger<ItemService> logger, IItemRepository itemRepository,
            ISpellRepository spellRepository)
        {
            this.logger = logger;
            this.itemRepository = itemRepository;
            this.spellRepository = spellRepository;
        }

        public override async Task<QueryResultReply> GetItem(QueryRequest request, ServerCallContext context)
        {
            var itemDb = await itemRepository.QueryOneAsync(new Infrastructure.Entities.Item()
            {
                ItemId = request.Id
            });

            if (itemDb == null)
            {
                logger.LogInformation($"Item not found => {request.Id}");
                return null;
            }

            return new QueryResultReply()
            {
                Value = itemDb.Value
            };
        }

        public override async Task<QueryMultipleResultReply> GetItems(QueryMultipleRequest request, ServerCallContext context)
        {
            var ids = request.Ids.ToArray();
            var itemsDb = await itemRepository.QueryMultipleByIdAsync(ids);

            var response = new QueryMultipleResultReply();
            response.Value.AddRange(itemsDb.Select(s => s.Value).ToArray());

            return response;
        }

        public override async Task<QueryResultReply> GetSpell(QueryRequest request, ServerCallContext context)
        {
            var spellDb = await spellRepository.QueryOneAsync(new Infrastructure.Entities.Spell()
            {
                SpellId = request.Id
            });

            if (spellDb == null)
            {
                logger.LogInformation($"Item not found => {request.Id}");
                return null;
            }

            return new QueryResultReply()
            {
                Value = spellDb.Value
            };
        }

        public override async Task<QueryMultipleResultReply> GetSpells(QueryMultipleRequest request, ServerCallContext context)
        {
            logger.LogInformation($"Receive ids : {string.Join(',', request.Ids.ToArray())}");
            var ids = request.Ids.ToArray();
            var spellsDb = await spellRepository.QueryMultipleByIdAsync(ids);

            var response = new QueryMultipleResultReply();
            response.Value.AddRange(spellsDb.Select(s => s.Value).ToArray());

            return response;
        }
    }
}
