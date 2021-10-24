using Infrastructure.Core.Persistence;

namespace Service.Item.Api.Infrastructure.Entities
{
    public class ItemNotFound : CoreEntity
    {
        public int ItemId { get; set; }
    }
}
