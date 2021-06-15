using Infrastructure.Core.Persistence;

namespace Worker.Infrastructure.Entities
{
    public class Spell : CoreEntity
    {
        public int SpellId { get; set; }

        public string NameFrFr { get; set; }

        public string NameEnUs { get; set; }

        public string NameEnGb { get; set; }

        public string DescriptionFrFr { get; set; }

        public string DescriptionEnUs { get; set; }

        public string DescriptionEnGb { get; set; }

        public string Value { get; set; }
    }
}
