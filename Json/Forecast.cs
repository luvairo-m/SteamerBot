using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Steamer
{
    internal class Forecast
    {
        [JsonPropertyName("main")]
        public IDictionary<string, double> Summary { get; set; }
        public IList<WeatherInfo> Weather { get; set; }
        public IDictionary<string, double> Wind { get; set; }
        public IDictionary<string, int> Clouds { get; set; }
        [JsonPropertyName("sys")]
        public CountryInfo Country { get; set; }

        internal class WeatherInfo
        {
            public string Main { get; set; }
            public string Description { get; set; }
        }

        internal class CountryInfo
        {
            [JsonPropertyName("country")]
            public string CountryName { get; set; }
            public int Sunset { get; set; }
            public int Sunrise { get; set; }
        }
    }
}