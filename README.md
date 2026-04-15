# MeteoApp

Application Windows pour consulter la météo.

## Fonctionnalités

- Recherche de la météo par ville
- Affichage de la température, description, humidité, vent et nuages
- Prévisions sur 5 jours
- Liste de villes favorites (sauvegardées automatiquement)
- Carte interactive avec la position de la ville
- Mode sombre et mode clair
- Interface sans bordures

## Comment l'utiliser

1. Ouvre le projet avec Visual Studio
2. Appuie sur F5 pour lancer l'application

Depuis un temrinal sur le projet, faire ces commandes : 

1. dotnet build 
2. dotnet run

Taper le nom d'une ville et appuyer sur Entrée ou cliquer sur le bouton de recherche.

Les favoris se sauvegardent automatiquement dans le fichier favorites.json.

## Executable

Afin de pouvoir obtenir un executable faire cette commande :

1. dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
2. Lancer l'executable MeteoApp.exe dans bin\Release\net8.0-windows\win-x64\publish

## Technologies

- C# et WinForms
- WebView2 pour la carte
- API OpenWeatherMap

## Remarques

- La clé API OpenWeather est déjà incluse dans le code.
- L'application est en français.
- La fenêtre est en taille fixe (1420x960).

Clé d'API modifiable dans app.Settings.json, Form1.cs et WeatherService.cs
