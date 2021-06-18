using Blizzard.WoWClassic.ApiContract.Items;
using System.Threading.Tasks;

namespace Worker.Interfaces
{
    public interface IItemBusiness
    {
        Task<ItemDetails[]> Search(string search);
    }
}
