using System.Text.Json.Serialization;

namespace Service.Item.Api.Contracts.Request
{
    public class QueryItemRequest
    {
        [JsonPropertyName("search")]
        public string Search { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }
    }
}
