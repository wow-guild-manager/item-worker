using Blizzard.WoWClassic.ApiContract.Items;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Service.Item.Api.Interfaces;

namespace Service.Item.Api.Business
{
    public class ItemBusiness : IItemBusiness
    {
        private readonly ILogger<ItemBusiness> logger;
        private readonly IItemRepository itemRepository;

        public ItemBusiness(ILogger<ItemBusiness> logger, IItemRepository itemRepository)
        {
            this.logger = logger;
            this.itemRepository = itemRepository;
        }

        public async Task<ItemDetails[]> Search(string search)
        {
            var itemsDatabase = await itemRepository.QueryMultipleAsync(new Contracts.Request.QueryItemRequest()
            {
                Search = search
            });

            return itemsDatabase.Select(s => JsonSerializer.Deserialize<ItemDetails>(s.Value)).ToArray();
        }
    }
}
