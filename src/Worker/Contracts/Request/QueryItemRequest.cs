using System.Text.Json.Serialization;

namespace Worker.Contracts.Request
{
    public class QueryItemRequest
    {
        [JsonPropertyName("search")]
        public string Search { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }
    }
}
