using Infrastructure.Core.Persistence;

namespace Worker.Infrastructure.Entities
{
    public class ItemNotFound : CoreEntity
    {
        public int ItemId { get; set; }
    }
}
