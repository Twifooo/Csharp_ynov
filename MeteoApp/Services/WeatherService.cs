using MeteoApp.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MeteoApp.Services
{
    public class WeatherService
    {
        private readonly string apiKey = "fc5355309cca05c952ee906bcac03878";
        private readonly HttpClient client = new HttpClient();

        public async Task<(double lat, double lon, string name)> GetCoordinatesAsync(string cityName)
        {
            string url = $"http://api.openweathermap.org/geo/1.0/direct?q={Uri.EscapeDataString(cityName)}&limit=1&appid={apiKey}";
            var response = await client.GetStringAsync(url);
            var data = JsonConvert.DeserializeObject<dynamic>(response);

            if (data == null || data.Count == 0)
                throw new Exception($"Ville '{cityName}' non trouvée.");

            return ((double)data[0].lat, (double)data[0].lon, (string)data[0].name);
        }

        public async Task<CurrentWeather> GetCurrentWeatherAsync(double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&lang=fr&appid={apiKey}";
            var json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<CurrentWeather>(json)!;
        }

        public async Task<ForecastResponse> GetForecastAsync(double lat, double lon)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&units=metric&lang=fr&appid={apiKey}";
            var json = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<ForecastResponse>(json)!;
        }

        public string GetIconUrl(string icon) => $"https://openweathermap.org/img/wn/{icon}@2x.png";
    }
}