using Infrastructure.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        public ItemSearchController(IItemBusiness itemBusiness, ILogger<ItemSearchController> logger)
        {
            this.itemBusiness = itemBusiness;
            this.logger = logger;
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
