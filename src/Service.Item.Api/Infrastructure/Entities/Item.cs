using Infrastructure.Core.Persistence;

namespace Service.Item.Api.Infrastructure.Entities
{
    public class Item : CoreEntity
    {
        public int ItemId { get; set; }

        public string NameFrFr { get; set; }

        public string NameEnUs { get; set; }

        public string NameEnGb { get; set; }

        public string Quality { get; set; }

        public string ItemClass { get; set; }

        public string ItemSubClass { get; set; }

        public string InventoryType { get; set; }

        public string Binding { get; set; }

        public string Value { get; set; }
    }
}
