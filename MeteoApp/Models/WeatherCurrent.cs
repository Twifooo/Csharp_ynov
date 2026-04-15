using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeteoApp.Models
{
    public class Coord
    {
        public double Lon { get; set; }
        public double Lat { get; set; }
    }

    public class Weather
    {
        public string? Description { get; set; }   // ← ajout du ?
        public string? Icon { get; set; }          // ← ajout du ?
    }

    public class Main
    {
        public double Temp { get; set; }
        public int Humidity { get; set; }
    }

    public class Wind
    {
        public double Speed { get; set; }
    }

    public class Clouds
    {
        public int All { get; set; }
    }

    public class CurrentWeather
    {
        public Coord? Coord { get; set; }
        public List<Weather>? Weather { get; set; }
        public Main? Main { get; set; }
        public Wind? Wind { get; set; }
        public Clouds? Clouds { get; set; }
        public string? Name { get; set; }
    }
}