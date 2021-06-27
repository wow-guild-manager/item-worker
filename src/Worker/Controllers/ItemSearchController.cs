using Infrastructure.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Worker.Interfaces;

namespace Worker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ItemSearchController : CoreController
    {
        private readonly IItemBusiness itemBusiness;
        private readonly ILogger<ItemSearchController> logger;
        private readonly ISpellRepository spellRepository;

        public ItemSearchController(IItemBusiness itemBusiness, ILogger<ItemSearchController> logger, ISpellRepository spellRepository)
        {
            this.itemBusiness = itemBusiness;
            this.logger = logger;
            this.spellRepository = spellRepository;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            var spellsDb = await spellRepository.QueryMultipleByIdAsync(new int[] { 1, 4, 24, 12, 18, 2448 });
            return Ok(spellsDb.Select(s => s.Value).ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> SearchItemsAsync([FromQuery] string search)
        {
            try
            {
                var result = await itemBusiness.Search(search);
                return Ok(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error on search", null);
            }

            return BadRequest();
        }
    }
}
