using MeteoApp.Models;
using MeteoApp.Services;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeteoApp
{
    public partial class Form1 : Form
    {
        private readonly WeatherService weatherService = new WeatherService();
        private readonly FavoritesService favoritesService = new FavoritesService();
        private List<string> favorites = new List<string>();

        private TextBox txtVille;
        private Button btnRechercher;
        private Button btnClose;
        private PictureBox picIcone;
        private Label lblTemperature, lblDescription, lblHumidite, lblVent, lblNuages;
        private FlowLayoutPanel flowPrevisions;
        private ListBox lstFavoris;
        private Button btnAjouterFavori, btnSupprimerFavori, btnMode;

        private Panel weatherPanel;
        private Label forecastLabel;
        private Label favLabel;

        // === MAP ===
        private WebView2 mapView;

        private const string OpenWeatherApiKey = "fc5355309cca05c952ee906bcac03878";

        private bool isDarkMode = true;
        private Color bgColorLight = Color.FromArgb(240, 245, 250);
        private Color bgColorDark = Color.FromArgb(15, 20, 35);
        private Color cardColorLight = Color.FromArgb(255, 255, 255);
        private Color cardColorDark = Color.FromArgb(25, 35, 60);
        private Color textColorLight = Color.FromArgb(30, 30, 30);
        private Color textColorDark = Color.FromArgb(255, 255, 255);
        private Color accentColor = Color.FromArgb(0, 120, 215);

        public Form1()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.DoubleBuffered = true;
            this.Size = new Size(1420, 960);
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeFormControls();
            LoadFavorites();
            ApplyTheme(isDarkMode);

            _ = InitializeMapAsync();
        }

        private void InitializeFormControls()
        {
            this.Text = "Météo";

            txtVille = new TextBox
            {
                Location = new Point(50, 20),
                Size = new Size(750, 50),
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15, 10, 15, 10),
                Text = ""
            };
            txtVille.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) _ = Rechercher(); };

            btnRechercher = new Button
            {
                Text = "🔍",
                Location = new Point(820, 20),
                Size = new Size(50, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 16),
                Cursor = Cursors.Hand
            };
            btnRechercher.FlatAppearance.BorderSize = 0;
            btnRechercher.Click += async (s, e) => await Rechercher();

            btnMode = new Button
            {
                Text = "☀️",
                Location = new Point(890, 20),
                Size = new Size(50, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 18),
                Cursor = Cursors.Hand
            };
            btnMode.FlatAppearance.BorderSize = 0;
            btnMode.Click += (s, e) => ToggleMode();

            btnClose = new Button
            {
                Text = "✕",
                Location = new Point(1350, 20),
                Size = new Size(50, 50),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 20),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            weatherPanel = new Panel
            {
                Name = "weatherPanel",
                Location = new Point(50, 90),
                Size = new Size(880, 420),
                Padding = new Padding(25)
            };

            picIcone = new PictureBox
            {
                Location = new Point(30, 30),
                Size = new Size(150, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            lblTemperature = new Label
            {
                Location = new Point(210, 40),
                Font = new Font("Segoe UI", 64, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblDescription = new Label
            {
                Location = new Point(210, 180),
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblHumidite = new Label
            {
                Location = new Point(30, 220),
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblVent = new Label
            {
                Location = new Point(30, 265),
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lblNuages = new Label
            {
                Location = new Point(30, 310),
                Font = new Font("Segoe UI", 13),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            weatherPanel.Controls.AddRange(new Control[] { picIcone, lblTemperature, lblDescription, lblHumidite, lblVent, lblNuages });

            forecastLabel = new Label
            {
                Text = "Prévisions 5 jours",
                Location = new Point(50, 530),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            flowPrevisions = new FlowLayoutPanel
            {
                Location = new Point(50, 560),
                Size = new Size(880, 340),
                AutoScroll = false,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(5)
            };

            // ===== FAVORITES PANEL =====
            favLabel = new Label
            {
                Text = "Favoris",
                Location = new Point(970, 90),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            lstFavoris = new ListBox
            {
                Location = new Point(970, 130),
                Size = new Size(400, 280),
                Font = new Font("Segoe UI", 12),
                BorderStyle = BorderStyle.None,
                ItemHeight = 32
            };
            lstFavoris.DoubleClick += async (s, e) =>
            {
                if (lstFavoris.SelectedItem is string city)
                {
                    txtVille.Text = city;
                    await Rechercher();
                }
            };

            btnAjouterFavori = new Button
            {
                Text = "+ Ajouter",
                Location = new Point(970, 420),
                Size = new Size(190, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAjouterFavori.FlatAppearance.BorderSize = 0;
            btnAjouterFavori.Click += (s, e) => AjouterFavori();

            btnSupprimerFavori = new Button
            {
                Text = "- Retirer",
                Location = new Point(1180, 420),
                Size = new Size(190, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSupprimerFavori.FlatAppearance.BorderSize = 0;
            btnSupprimerFavori.Click += (s, e) => SupprimerFavori();

            // ===== MAP PANEL =====
            mapView = new WebView2
            {
                Location = new Point(970, 500),
                Size = new Size(400, 400),
                Visible = true
            };

            // ===== ADD ALL CONTROLS =====
            this.Controls.AddRange(new Control[]
            {
                txtVille, btnRechercher, btnMode, btnClose,
                weatherPanel, forecastLabel, flowPrevisions,
                favLabel, lstFavoris, btnAjouterFavori, btnSupprimerFavori,
                mapView
            });
        }

        private async Task InitializeMapAsync()
        {
            try
            {
                await mapView.EnsureCoreWebView2Async();
                mapView.DefaultBackgroundColor = Color.Transparent;

                LoadMap(48.8566, 2.3522, "Paris");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'initialiser la carte : " + ex.Message,
                    "Erreur WebView2",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadMap(double lat, double lon, string cityName)
        {
            if (mapView.CoreWebView2 == null)
                return;

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta http-equiv='X-UA-Compatible' content='IE=edge' />
    <meta name='viewport' content='width=device-width, initial-scale=1.0' />

    <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.4/dist/leaflet.css'/>
    <script src='https://unpkg.com/leaflet@1.9.4/dist/leaflet.js'></script>

    <style>
        html, body {{
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            overflow: hidden;
            background: transparent;
        }}

        #map {{
            width: 100%;
            height: 100%;
        }}

        .leaflet-control-attribution {{
            font-size: 10px;
        }}
    </style>
</head>
<body>
    <div id='map'></div>

    <script>
        const lat = {lat.ToString(System.Globalization.CultureInfo.InvariantCulture)};
        const lon = {lon.ToString(System.Globalization.CultureInfo.InvariantCulture)};

        const map = L.map('map', {{
            zoomControl: true
        }}).setView([lat, lon], 8);

        // Fond de carte OpenStreetMap
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            maxZoom: 19,
            attribution: '&copy; OpenStreetMap contributors'
        }}).addTo(map);

        // Couche météo OpenWeather
        // Exemples de layers possibles : TA2 (temp), PR0 (précipitation), WS10 (vent), CL (nuages)
        L.tileLayer('https://maps.openweathermap.org/maps/2.0/weather/TA2/{{z}}/{{x}}/{{y}}?appid={OpenWeatherApiKey}', {{
            opacity: 0.6,
            attribution: '&copy; OpenWeather'
        }}).addTo(map);

        L.marker([lat, lon]).addTo(map)
            .bindPopup('{cityName.Replace("'", "\\'")}')
            .openPopup();
    </script>
</body>
</html>";

            mapView.NavigateToString(html);
        }

        private async Task Rechercher()
        {
            string city = txtVille.Text.Trim();
            if (string.IsNullOrEmpty(city)) return;

            try
            {
                var (lat, lon, name) = await weatherService.GetCoordinatesAsync(city);
                var current = await weatherService.GetCurrentWeatherAsync(lat, lon);
                var forecast = await weatherService.GetForecastAsync(lat, lon);

                var w = current.Weather?[0];
                if (w == null) return;

                picIcone.LoadAsync(weatherService.GetIconUrl(w.Icon ?? ""));
                lblTemperature.Text = $"{current.Main?.Temp:F1}°";
                lblDescription.Text = char.ToUpper(w.Description?[0] ?? ' ') + (w.Description?.Substring(1) ?? "");

                lblHumidite.Text = $"💧 Humidité: {current.Main?.Humidity}%";
                lblVent.Text = $"🌬 Vent: {current.Wind?.Speed:F1} m/s";
                lblNuages.Text = $"☁ Nuages: {current.Clouds?.All}%";

                DisplayForecasts(forecast);

                // Mise à jour de la carte
                LoadMap(lat, lon, name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayForecasts(ForecastResponse forecast)
        {
            flowPrevisions.Controls.Clear();
            if (forecast.List == null) return;

            for (int i = 0; i < forecast.List.Count && i < 40; i += 8)
            {
                flowPrevisions.Controls.Add(CreateForecastCard(forecast.List[i]));
            }
        }

        private Panel CreateForecastCard(ForecastItem item)
        {
            var date = DateTimeOffset.FromUnixTimeSeconds(item.Dt).ToLocalTime();

            var card = new Panel
            {
                Size = new Size(160, 310),
                Margin = new Padding(5),
                Padding = new Padding(10),
                Cursor = Cursors.Hand
            };

            card.MouseEnter += (s, e) => card.BackColor = isDarkMode ? Color.FromArgb(45, 55, 85) : Color.FromArgb(230, 240, 250);
            card.MouseLeave += (s, e) => card.BackColor = isDarkMode ? Color.FromArgb(25, 35, 60) : Color.FromArgb(255, 255, 255);

            var pic = new PictureBox
            {
                Size = new Size(90, 90),
                Location = new Point(35, 5),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            if (item.Weather?.Count > 0)
                pic.LoadAsync(weatherService.GetIconUrl(item.Weather[0].Icon ?? ""));

            var lblDate = new Label
            {
                Text = date.ToString("ddd\ndd/MM"),
                Location = new Point(5, 105),
                Size = new Size(150, 40),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            var lblTemp = new Label
            {
                Text = $"{item.Main?.Temp:F0}°",
                Location = new Point(5, 150),
                Size = new Size(150, 35),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            string desc = item.Weather?[0]?.Description ?? "";
            var lblDesc = new Label
            {
                Text = TruncateText(desc.ToUpper(), 15),
                Location = new Point(5, 190),
                Size = new Size(150, 110),
                TextAlign = ContentAlignment.TopCenter,
                Font = new Font("Segoe UI", 7, FontStyle.Regular),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            card.Controls.AddRange(new Control[] { pic, lblDate, lblTemp, lblDesc });
            return card;
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength) + "...";
        }

        private void ToggleMode()
        {
            isDarkMode = !isDarkMode;
            ApplyTheme(isDarkMode);
            btnMode.Text = isDarkMode ? "☀️" : "🌙";
        }

        private void ApplyTheme(bool dark)
        {
            isDarkMode = dark;

            this.BackColor = dark ? bgColorDark : bgColorLight;

            txtVille.BackColor = dark ? cardColorDark : cardColorLight;
            txtVille.ForeColor = dark ? textColorDark : textColorLight;

            btnRechercher.BackColor = accentColor;
            btnRechercher.ForeColor = Color.White;

            btnMode.BackColor = dark ? Color.FromArgb(45, 55, 85) : Color.FromArgb(220, 230, 245);
            btnMode.ForeColor = dark ? textColorDark : textColorLight;

            btnClose.BackColor = dark ? Color.FromArgb(45, 55, 85) : Color.FromArgb(220, 230, 245);
            btnClose.ForeColor = dark ? textColorDark : textColorLight;

            if (weatherPanel != null)
            {
                weatherPanel.BackColor = dark ? cardColorDark : cardColorLight;
                foreach (Label lbl in weatherPanel.Controls.OfType<Label>())
                    lbl.ForeColor = dark ? textColorDark : textColorLight;
            }

            lstFavoris.BackColor = dark ? cardColorDark : cardColorLight;
            lstFavoris.ForeColor = dark ? textColorDark : textColorLight;

            btnAjouterFavori.BackColor = accentColor;
            btnAjouterFavori.ForeColor = Color.White;

            btnSupprimerFavori.BackColor = dark ? Color.FromArgb(100, 100, 120) : Color.FromArgb(200, 200, 220);
            btnSupprimerFavori.ForeColor = dark ? textColorDark : textColorLight;

            if (forecastLabel != null) forecastLabel.ForeColor = dark ? textColorDark : textColorLight;
            if (favLabel != null) favLabel.ForeColor = dark ? textColorDark : textColorLight;

            RefreshForecastCards();
        }

        private void RefreshForecastCards()
        {
            foreach (Panel card in flowPrevisions.Controls.OfType<Panel>())
            {
                card.BackColor = isDarkMode ? cardColorDark : cardColorLight;
                foreach (Label lbl in card.Controls.OfType<Label>())
                    lbl.ForeColor = isDarkMode ? textColorDark : textColorLight;
            }
        }

        private void AjouterFavori()
        {
            string city = txtVille.Text.Trim();
            if (!string.IsNullOrEmpty(city) && !favorites.Contains(city))
            {
                favorites.Add(city);
                lstFavoris.DataSource = null;
                lstFavoris.DataSource = new List<string>(favorites);
                favoritesService.SaveFavorites(favorites);
            }
        }

        private void SupprimerFavori()
        {
            if (lstFavoris.SelectedItem is string city)
            {
                favorites.Remove(city);
                lstFavoris.DataSource = null;
                lstFavoris.DataSource = new List<string>(favorites);
                favoritesService.SaveFavorites(favorites);
            }
        }

        private void LoadFavorites()
        {
            favorites = favoritesService.LoadFavorites();
            lstFavoris.DataSource = new List<string>(favorites);
        }
    }
}