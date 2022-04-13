using Blizzard.WoWClassic.ApiClient;
using Blizzard.WoWClassic.ApiClient.Exceptions;
using Blizzard.WoWClassic.ApiClient.Helpers;
using Blizzard.WoWClassic.ApiContract.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Service.Item.Api.Infrastructure.Entities;
using Service.Item.Api.Interfaces;

namespace Service.Item.Api.Services
{
    public class ItemPullerService : BackgroundService
    {
        private static List<int> itemNotFoundId = new List<int>();

        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<ItemPullerService> logger;

        public ItemPullerService(IServiceProvider serviceProvider, ILogger<ItemPullerService> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            if (!Directory.Exists("Json"))
                Directory.CreateDirectory("Json");
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

                logger.LogInformation($"Items not found {itemNotFoundId.Count}");
            }

            for (var i = 1; i < 85000; i++)
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
                    logger.LogError(e, $"Error ... {e.Message}");
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
                            itemDb.UpdateAt = DateTime.UtcNow;
                            itemDb.UpdateBy = "System Service.Item.Api";

                            await itemRepository.InsertAsync(itemDb);
                        }

                        // await File.WriteAllTextAsync($"Json/item_{itemId}.json", JsonSerializer.Serialize(itemDetails));
                        await ProcessSpellAsync(itemDetails.PreviewItem.Spells);
                    }

                    var itemMedia = await clientWow.GetItemMediaAsync(itemId, RegionHelper.Us, NamespaceHelper.Static);

                    if (itemMedia != null && itemMedia.Assets.Any())
                    {
                        var downloadResult = await clientWow.DownloadMediaAsync(itemMedia.Assets[0].Value, $"{itemId}.png");
                        File.WriteAllBytes($"Medias/item_{itemId}.png", downloadResult.Data);
                    }
                }
                catch (ApiException ea)
                {
                    if (!itemNotFoundId.Contains(itemId))
                    {
                        itemNotFoundId.Add(itemId);
                        await itemNotFoundRepository.InsertAsync(new ItemNotFound()
                        {
                            CreateBy = "System Service.Item.Api",
                            Id = Guid.NewGuid(),
                            ItemId = itemId,
                            CreateAt = DateTime.UtcNow
                        });
                    }

                    logger.LogError(ea, $"Item id not found => {itemId}.");
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"Error ... {e.Message} => Item ID : {itemId} not insert.");
                }
            }
        }

        private async Task ProcessSpellAsync(SpellValue<ValueLocale>[] spells)
        {
            if (spells != null)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var spellRepository =
                            scope.ServiceProvider
                            .GetRequiredService<ISpellRepository>();

                    foreach (var spell in spells)
                    {
                        try
                        {
                            var spellDb = await spellRepository.QueryOneAsync(new Spell()
                            {
                                SpellId = spell.Spell.Id
                            });

                            if (spellDb == null)
                            {
                                var newSpell = Map(spell);
                                await spellRepository.InsertAsync(newSpell);
                            }
                            else
                            {
                                var spellToUpdate = Map(spell);

                                spellToUpdate.Id = spellDb.Id;
                                spellToUpdate.CreateAt = spellDb.CreateAt;
                                spellToUpdate.CreateBy = spellDb.CreateBy;
                                spellToUpdate.UpdateAt = DateTime.UtcNow;
                                spellToUpdate.UpdateBy = "System Service.Item.Api";

                                await spellRepository.UpdateAsync(spellDb);
                            }

                        }
                        catch (Exception e)
                        {
                            logger.LogError(e, $"Error ... {e.Message} => Spell ID : {spell.Spell.Id} not insert.");
                        }
                    }
                }
            }
        }

        private Service.Item.Api.Infrastructure.Entities.Item Map(Blizzard.WoWClassic.ApiContract.Items.ItemDetails itemDetails)
        {
            return new Service.Item.Api.Infrastructure.Entities.Item()
            {
                CreateAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                ItemId = itemDetails.Id,
                NameFrFr = itemDetails.Name.FrFR,
                NameEnGb = itemDetails.Name.EnGb,
                NameEnUs = itemDetails.Name.EnUs,
                Quality = itemDetails.PreviewItem.Quality.Type,
                InventoryType = itemDetails.PreviewItem.InventoryType.Type,
                ItemClass = itemDetails.ItemClass?.Name?.EnUs,
                ItemSubClass = itemDetails.ItemSubclass?.Name?.EnUs,
                Binding = itemDetails.PreviewItem?.Binding?.Name?.EnUs,
                Value = JsonSerializer.Serialize(itemDetails),
                CreateBy = "System Service.Item.Api"
            };
        }

        private Service.Item.Api.Infrastructure.Entities.Spell Map(Blizzard.WoWClassic.ApiContract.Core.SpellValue<ValueLocale> spellDetails)
        {
            return new Service.Item.Api.Infrastructure.Entities.Spell()
            {
                Id = Guid.NewGuid(),
                SpellId = spellDetails.Spell.Id,
                CreateAt = DateTime.UtcNow,
                CreateBy = "System Service.Item.Api",
                NameFrFr = spellDetails.Spell.Name.FrFR,
                NameEnGb = spellDetails.Spell.Name.EnGb,
                NameEnUs = spellDetails.Spell.Name.EnUs,
                Value = JsonSerializer.Serialize(spellDetails),
                DescriptionFrFr = spellDetails.Description.FrFR,
                DescriptionEnUs = spellDetails.Description.EnUs,
                DescriptionEnGb = spellDetails.Description.EnGb
            };
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
