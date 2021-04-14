using Azure.Storage.Blobs;
using Blizzard.WoWClassic.ApiClient;
using Blizzard.WoWClassic.ApiClient.Exceptions;
using Blizzard.WoWClassic.ApiClient.Helpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Worker.Services
{
    public class ItemPullerService : IHostedService, IDisposable
    {
        private string connectionString = "DefaultEndpointsProtocol=https;AccountName=appepicstorage;AccountKey=Cyr196oHhMjTTnCgCV1A/LXADKlxzwI+v5+NTxp2wzq9bkgFUDK5wB0337gC7CrRfz712vHJD9UVnwe3tXbQaA==;EndpointSuffix=core.windows.net";

        private static List<int> itemNotFoundId = new List<int>();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private async Task Extract()
        {
            var clientWow = new WoWClassicApiClient("bxSvhNNHJwI0kgNvKy6Z91oMEOpwgjmv", "2b136112d3064b11b19c5ea275846996");
            clientWow.SetDefaultValues(RegionHelper.Europe, NamespaceHelper.Dynamic, LocaleHelper.French);

            if (File.Exists("notfoundId.json"))
            {
                itemNotFoundId = JsonSerializer.Deserialize<List<int>>(await File.ReadAllTextAsync("notfoundId.json"));
            }

            for (var i = 1; i < 50000; i++)
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
            try
            {
                var itemDetails = await clientWow.GetItemDetailsAsync(itemId, RegionHelper.Us, NamespaceHelper.Static);

                if (itemDetails != null)
                {
                    await UploadToAzureItem($"{itemId}.json", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(itemDetails)));
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
                    await File.WriteAllTextAsync("notfoundId.json", JsonSerializer.Serialize(itemNotFoundId));
                    itemNotFoundId.Add(itemId);
                }

                Console.WriteLine($"Item id not found {itemId}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error ... {e.Message}");
            }
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Extract();
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
