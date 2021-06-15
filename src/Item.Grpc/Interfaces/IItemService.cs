using Blizzard.WoWClassic.ApiContract.Core;
using Blizzard.WoWClassic.ApiContract.Items;
using System.Threading.Tasks;
using WGM.Service.Item;

namespace Item.Grpc.Interfaces
{
    public interface IItemService
    {
        Task<ItemDetails[]> GetItemsAsync(QueryMultipleRequest request);
        Task<ItemDetails> GetItemsAsync(QueryRequest request);
        Task<SpellValue<ValueLocale>[]> GetSpellsAsync(QueryMultipleRequest request);
        Task<SpellValue<ValueLocale>> GetSpellAsync(QueryRequest request);
    }
}
