using Blizzard.WoWClassic.ApiContract.Core;
using Blizzard.WoWClassic.ApiContract.Items;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure.Core.Interfaces;
using Item.Grpc.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WGM.Service.Item;

namespace Item.Grpc.Services
{
    public class ItemService : IItemService
    {
        private readonly ILogger<ItemService> logger;
        private readonly ItemGrpcService.ItemGrpcServiceClient client;
        private readonly IUriResolver uriResolver;

        public ItemService(ILogger<ItemService> logger, IUriResolver uriResolver)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(uriResolver.Resolve("service-item"));
            this.client = new ItemGrpcService.ItemGrpcServiceClient(channel);
            this.logger = logger;
            this.uriResolver = uriResolver;
        }

        public async Task<ItemDetails[]> GetItemsAsync(QueryMultipleRequest request)
        {
            try
            {
                var queryResult = await client.GetItemsAsync(request);
                return queryResult.Value.Select(s => JsonSerializer.Deserialize<ItemDetails>(s)).ToArray();
            }
            catch (RpcException ex)
            {
                logger.LogCritical(ex, $"gRPC exception throw.");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Exception on gRPC client.");
            }

            return default(ItemDetails[]);
        }

        public async Task<ItemDetails> GetItemAsync(QueryRequest request)
        {
            try
            {
                var queryResult = await client.GetItemAsync(request);
                return JsonSerializer.Deserialize<ItemDetails>(queryResult.Value);
            }
            catch (RpcException ex)
            {
                logger.LogCritical(ex, $"gRPC exception throw.");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Exception on gRPC client.");
            }

            return default(ItemDetails);
        }

        public async Task<SpellValue<ValueLocale>[]> GetSpellsAsync(QueryMultipleRequest request)
        {
            try
            {
                var queryResult = await client.GetSpellsAsync(request);
                return queryResult.Value.Select(s => JsonSerializer.Deserialize<SpellValue<ValueLocale>>(s)).ToArray();
            }
            catch (RpcException ex)
            {
                logger.LogCritical(ex, $"gRPC exception throw.");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Exception on gRPC client.");
            }

            return default(SpellValue<ValueLocale>[]);
        }

        public async Task<SpellValue<ValueLocale>> GetSpellAsync(QueryRequest request)
        {
            try
            {
                var queryResult = await client.GetSpellAsync(request);
                return JsonSerializer.Deserialize<SpellValue<ValueLocale>>(queryResult.Value);
            }
            catch (RpcException ex)
            {
                logger.LogCritical(ex, $"gRPC exception throw.");
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Exception on gRPC client.");
            }

            return default(SpellValue<ValueLocale>);
        }
    }
}
