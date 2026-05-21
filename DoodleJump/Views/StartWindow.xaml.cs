using System;
using System.Windows;
using System.Windows.Controls;
using Model.Data;
using System.IO;
using System.Linq;

namespace DoodleJump.Views
{
    public partial class StartWindow : Window
    {
        private ScoreManager _scoreManager = new();
        private AppConfig _config = null!;
        private JsonGameSerializer<AppConfig> _configSer = new();

        public StartWindow()
{
    InitializeComponent();
    LoadOrCreateConfig(); // Загружаем config.json
    LoadHighScores();
    CheckSaveFile(); // Теперь он проверит путь из _config.SavePath
}

private void LoadOrCreateConfig()
{
    if (File.Exists("config.json")) {
        _config = _configSer.Deserialize("config.json");
    } else {
        _config = new AppConfig { SavePath = "saves/" }; // По умолчанию
        SaveConfig();
    }
}

        

        private void SaveConfig() => _configSer.Serialize("config.json", _config);

        private void LoadHighScores()
        {
            // Загружаем упрощенный список (Score и Difficulty)
            GridScores.ItemsSource = _scoreManager.Load();
        }

        private void CheckSaveFile()
        {
            // Берем формат и путь из конфига, а не из UI
            string ext = _config.UseXml ? ".xml" : ".json";
            string fullPath = System.IO.Path.Combine(_config.SavePath, "savegame" + ext);
            BtnContinue.IsEnabled = File.Exists(fullPath);
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            string ext = _config.UseXml ? ".xml" : ".json";
            string fullPath = System.IO.Path.Combine(_config.SavePath, "savegame" + ext);
            if (File.Exists(fullPath)) File.Delete(fullPath);

            StartGame(null);
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            StartGame(_config.SavePath);
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e) => OpenSettings();

        private void OpenSettings()
        {
            var settingsWin = new SettingsWindow(_config);
            settingsWin.Owner = this;
            if (settingsWin.ShowDialog() == true)
            {
                CheckSaveFile();
            }
        }

        private void StartGame(string? loadPath)
        {
            // Передаем настройки из конфига и слайдера сложности
            MainWindow gameWindow = new MainWindow(loadPath, _config.UseXml, (int)SliderDifficulty.Value);
            gameWindow.Show();
            this.Close();
        }
        private void BtnExitApp_Click(object sender, RoutedEventArgs e)
{
    Application.Current.Shutdown(); // Полное закрытие программы
}
    }
}