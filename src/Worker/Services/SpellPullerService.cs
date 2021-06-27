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
                var url = $"https://tbc.wowhead.com/spell={id}";
                var web = new HtmlWeb();
                var doc = web.Load(url);

                var spellName = doc.DocumentNode
                                .SelectSingleNode("//h1[@class='heading-size-1']")?.InnerText;

                if (spellName == "Spells")
                {
                    Console.WriteLine($"Spell not found ... {id}");
                    return;
                }

                var spellDescription = doc.DocumentNode
                                        .SelectSingleNode("//td/div[@class='q']")?.InnerText;

                if (string.IsNullOrEmpty(spellDescription))
                    spellDescription = spellName;

                var spell = new SpellValue<ValueLocale>()
                {
                    Spell = new SpellDetailsValue<ValueLocale>()
                    {
                        Id = id,
                        Key = new LinkItem()
                        {
                            Href = url
                        },
                        Name = new ValueLocale()
                        {
                            EnGb = spellName,
                            EnUs = spellName,
                            FrFR = spellName
                        }
                    },
                    Description = new ValueLocale()
                    {
                        EnGb = spellName,
                        EnUs = spellName,
                        FrFR = spellName
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
