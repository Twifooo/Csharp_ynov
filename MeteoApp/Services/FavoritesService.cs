using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MeteoApp.Services
{
    public class FavoritesService
    {
        private readonly string filePath = "favorites.json";

        public List<string> LoadFavorites()
        {
            if (!File.Exists(filePath))
                return new List<string>();

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<string>>(json) ?? new List<string>();
        }

        public void SaveFavorites(List<string> favorites)
        {
            string json = JsonConvert.SerializeObject(favorites, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
    }
}