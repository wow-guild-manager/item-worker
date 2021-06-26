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


                var newDbSpell = new Spell()
                {
                    CreateAt = DateTime.UtcNow,
                    CreateBy = "System worker",
                    DescriptionEnUs = spellDescription,
                    DescriptionFrFr = spellDescription,
                    DescriptionEnGb = spellDescription,
                    NameFrFr = spellName,
                    NameEnGb = spellName,
                    NameEnUs = spellName,
                    SpellId = id,
                    Value = JsonSerializer.Serialize(new
                    {
                        spell = new
                        {
                            name = new { en_US = spellName, en_GB = spellName, fr_FR = spellName },
                            id = id
                        },
                        key = new
                        {
                            href = url
                        },
                        description = new { en_US = spellDescription, en_GB = spellDescription, fr_FR = spellDescription }
                    })
                };

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
                        spellDb.UpdateAt = DateTime.UtcNow;
                        spellDb.UpdateBy = "System worker";

                        spellDb.NameFrFr = spellName;
                        spellDb.NameEnUs = spellName;
                        spellDb.NameEnGb = spellName;

                        spellDb.DescriptionFrFr = spellDescription;
                        spellDb.DescriptionEnUs = spellDescription;
                        spellDb.DescriptionEnGb = spellDescription;

                        spellDb.Value = JsonSerializer.Serialize(new
                        {
                            spell = new
                            {
                                name = new { en_US = spellName, en_GB = spellName, fr_FR = spellName },
                                id = id
                            },
                            key = new
                            {
                                href = url
                            },
                            description = new { en_US = spellDescription, en_GB = spellDescription, fr_FR = spellDescription }
                        });

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
    }
}
