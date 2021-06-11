using Azure.Storage.Blobs;
using Blizzard.WoWClassic.ApiClient;
using Blizzard.WoWClassic.ApiClient.Exceptions;
using Blizzard.WoWClassic.ApiClient.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;
using Worker.Interfaces;

namespace Worker.Services
{
    public class ItemPullerService : BackgroundService
    {
        private string connectionString = "DefaultEndpointsProtocol=https;AccountName=appepicstorage;AccountKey=Cyr196oHhMjTTnCgCV1A/LXADKlxzwI+v5+NTxp2wzq9bkgFUDK5wB0337gC7CrRfz712vHJD9UVnwe3tXbQaA==;EndpointSuffix=core.windows.net";

        private static List<int> itemNotFoundId = new List<int>();

        private readonly IServiceProvider serviceProvider;

        public ItemPullerService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private async Task Extract()
        {
            var clientWow = new WoWClassicApiClient("bxSvhNNHJwI0kgNvKy6Z91oMEOpwgjmv", "2b136112d3064b11b19c5ea275846996");
            clientWow.SetDefaultValues(RegionHelper.Europe, NamespaceHelper.Dynamic, LocaleHelper.French);

            using (var scope = serviceProvider.CreateScope())
            {
                var itemNotFoundRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IItemNotFoundRepository>();

                var itemsNotFoundDb = await itemNotFoundRepository.QueryMultipleAsync(new ItemNotFound());
                itemNotFoundId = itemsNotFoundDb.Select(s => s.ItemId).ToList();

                Console.WriteLine($"Items not found {itemNotFoundId.Count}");
            }

            for (var i = 1; i < 65000; i++)
            {
                try
                {
                    if (!itemNotFoundId.Contains(i))
                    {
                        await ExtractBlizzardDatabase(clientWow, i);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error ... {e.Message}");
                }
            }
        }

        private async Task ExtractBlizzardDatabase(WoWClassicApiClient clientWow, int itemId)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var itemRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IItemRepository>();

                var itemNotFoundRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IItemNotFoundRepository>();

                try
                {
                    var itemDetails = await clientWow.GetItemDetailsAsync(itemId, RegionHelper.Us, NamespaceHelper.Static);

                    if (itemDetails != null)
                    {
                        var itemExist = await itemRepository.QueryOneAsync(new Infrastructure.Entities.Item()
                        {
                            ItemId = itemId
                        });

                        if (itemExist != null)
                        {
                            var itemDb = Map(itemDetails);

                            itemDb.Id = itemExist.Id;
                            itemDb.CreateAt = itemExist.CreateAt;
                            itemDb.CreateBy = itemExist.CreateBy;

                            await itemRepository.UpdateAsync(itemDb);
                        }
                        else
                        {
                            var itemDb = Map(itemDetails);
                            await itemRepository.InsertAsync(itemDb);

                        }

                        //await UploadToAzureItem($"{itemId}.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(itemDetails)));
                    }

                    var itemMedia = await clientWow.GetItemMediaAsync(itemId, RegionHelper.Us, NamespaceHelper.Static);

                    if (itemMedia != null && itemMedia.Assets.Any())
                    {
                        var downloadResult = await clientWow.DownloadMediaAsync(itemMedia.Assets[0].Value, $"{itemId}.png");
                        await UploadToAzure($"item_{itemId}.png", downloadResult.Data);
                    }
                }
                catch (ApiException)
                {
                    if (!itemNotFoundId.Contains(itemId))
                    {
                        itemNotFoundId.Add(itemId);
                        await itemNotFoundRepository.InsertAsync(new ItemNotFound()
                        {
                            CreateBy = "System Worker",
                            Id = Guid.NewGuid(),
                            ItemId = itemId,
                            CreateAt = DateTime.UtcNow
                        });
                    }

                    Console.WriteLine($"Item id not found {itemId}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error ... {e.Message} => Item ID : {itemId}");
                }
            }
        }

        private Item Map(Blizzard.WoWClassic.ApiContract.Items.ItemDetails itemDetails)
        {
            return new Item()
            {
                CreateAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                ItemId = itemDetails.Id,
                NameFrFr = itemDetails.Name.FrFR,
                NameEnGb = itemDetails.Name.EnGb,
                NameEnUs = itemDetails.Name.EnUs,
                Quality = itemDetails.PreviewItem.Quality.Type,
                InventoryType = itemDetails.PreviewItem.InventoryType.Type,
                ItemClass = itemDetails.ItemClass.Name.EnUs,
                ItemSubClass = itemDetails.ItemSubclass.Name.EnUs,
                Value = JsonSerializer.Serialize(itemDetails),
                CreateBy = "System Worker"
            };
        }

        private string MapQuality(string type)
        {
            throw new NotImplementedException();
        }

        private async Task UploadToAzureItem(string fileName, byte[] data)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("items");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            MemoryStream stream = new MemoryStream(data);
            await blobClient.UploadAsync(stream, true);
            stream.Close();
        }

        private async Task UploadToAzure(string fileName, byte[] data)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("medias");
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            MemoryStream stream = new MemoryStream(data);
            await blobClient.UploadAsync(stream, true);
            stream.Close();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Extract();
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
