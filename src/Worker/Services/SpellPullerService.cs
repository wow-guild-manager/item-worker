using Blizzard.WoWClassic.ApiContract.Core;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Worker.Infrastructure.Entities;
using Worker.Interfaces;

namespace Worker.Services
{
    public class SpellPullerService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public SpellPullerService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private async Task Extract()
        {
            for (var i = 1; i < 20000; i++)
            {
                try
                {
                    await ExtractWowHeadSpellAsync(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error ... {e.Message}");
                }
            }
        }

        private async Task ExtractWowHeadSpellAsync(int id)
        {
            try
            {
                var urlEn = $"https://tbc.wowhead.com/spell={id}";
                var urlFr = $"https://fr.tbc.wowhead.com/spell={id}";
                var web = new HtmlWeb();
                var docEn = web.Load(urlEn);
                var docFr = web.Load(urlFr);

                var spellName = docEn.DocumentNode
                                .SelectSingleNode("//h1[@class='heading-size-1']")?.InnerText;
                var spellNameFr = docFr.DocumentNode
                                .SelectSingleNode("//h1[@class='heading-size-1']")?.InnerText;

                if (spellName == "Spells")
                {
                    Console.WriteLine($"Spell not found ... {id}");
                    return;
                }

                if (string.IsNullOrEmpty(spellNameFr))
                    spellNameFr = spellName;

                var spellDescription = docEn.DocumentNode
                                        .SelectSingleNode("//td/div[@class='q']")?.InnerText;
                var spellDescriptionFr = docFr.DocumentNode
                                        .SelectSingleNode("//td/div[@class='q']")?.InnerText;

                if (string.IsNullOrEmpty(spellDescription))
                    spellDescription = spellName;

                if (string.IsNullOrEmpty(spellDescriptionFr))
                    spellDescriptionFr = spellName;

                var spell = new SpellValue<ValueLocale>()
                {
                    Spell = new SpellDetailsValue<ValueLocale>()
                    {
                        Id = id,
                        Key = new LinkItem()
                        {
                            Href = urlEn
                        },
                        Name = new ValueLocale()
                        {
                            EnGb = spellName,
                            EnUs = spellName,
                            FrFR = spellNameFr
                        }
                    },
                    Description = new ValueLocale()
                    {
                        EnGb = spellDescription,
                        EnUs = spellDescription,
                        FrFR = spellDescriptionFr
                    }
                };
                var newDbSpell = Map(spell);

                using (var scope = serviceProvider.CreateScope())
                {
                    var spellRepository =
                        scope.ServiceProvider
                            .GetRequiredService<ISpellRepository>();

                    var spellDb = await spellRepository.QueryOneAsync(new Spell()
                    {
                        SpellId = id
                    });

                    if (spellDb == null)
                    {
                        _ = await spellRepository.InsertAsync(newDbSpell);
                    }
                    else
                    {
                        newDbSpell.UpdateAt = DateTime.UtcNow;
                        newDbSpell.UpdateBy = "System worker";
                        newDbSpell.Id = spellDb.Id;
                        newDbSpell.CreateAt = spellDb.CreateAt;
                        newDbSpell.CreateBy = spellDb.CreateBy;

                        _ = await spellRepository.UpdateAsync(spellDb);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error ... {e.Message}");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                await Extract();
                await Task.Delay(TimeSpan.FromDays(1));
            }
        }

        private Spell Map(SpellValue<ValueLocale> spellDetails)
        {
            return new Spell()
            {
                Id = Guid.NewGuid(),
                SpellId = spellDetails.Spell.Id,
                CreateAt = DateTime.UtcNow,
                CreateBy = "System Worker",
                NameFrFr = spellDetails.Spell.Name.FrFR,
                NameEnGb = spellDetails.Spell.Name.EnGb,
                NameEnUs = spellDetails.Spell.Name.EnUs,
                Value = JsonSerializer.Serialize(spellDetails),
                DescriptionFrFr = spellDetails.Description.FrFR,
                DescriptionEnUs = spellDetails.Description.EnUs,
                DescriptionEnGb = spellDetails.Description.EnGb
            };
        }
    }
}
