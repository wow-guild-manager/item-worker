using Blizzard.WoWClassic.ApiContract.Items;
using System.Threading.Tasks;

namespace Service.Item.Api.Interfaces
{
    public interface IItemBusiness
    {
        Task<ItemDetails[]> Search(string search);
    }
}
