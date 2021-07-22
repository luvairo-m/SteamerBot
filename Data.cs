using System.Text.Json.Serialization;

namespace Steamer
{
    internal class Data
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }
    }
}
