using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoApp.Models
{
    public class ForecastItem
    {
        public long Dt { get; set; }
        public Main? Main { get; set; }
        public List<Weather>? Weather { get; set; }
        public Wind? Wind { get; set; }
        public Clouds? Clouds { get; set; }
        public string? Dt_txt { get; set; }
    }

    public class ForecastResponse
    {
        public List<ForecastItem>? List { get; set; }
    }
}