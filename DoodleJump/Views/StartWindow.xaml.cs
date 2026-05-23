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
        private const string InfoFolder = "Game.info"; // Системная папка
        private const string ConfigPath = "Game.info/config.json"; // Файл настроек

        private ScoreManager _scoreManager = new(); // Управление рейтингом
        private AppConfig _config = null!; // Данные настроек (путь, формат)
        private JsonGameSerializer<AppConfig> _configSer = new(); // Загрузчик настроек
        private string _saveFileName = "savegame"; // Имя файла сессии

        public StartWindow() // Запуск меню
        {
            InitializeComponent();
            LoadOrCreateConfig(); // Загрузка настроек из Game.info
            LoadHighScores(); // Показ рейтинга
            CheckSaveFile(); // Проверка кнопки "Продолжить"
        }

        // Чтение файла настроек из системной папки
        private void LoadOrCreateConfig()
        {
            if (!Directory.Exists(InfoFolder)) Directory.CreateDirectory(InfoFolder); // Создаем системную папку

            if (File.Exists(ConfigPath)) {
                _config = _configSer.Deserialize(ConfigPath); // Загружаем конфиг
            } else {
                // Первый запуск: создаем папку Game.saves и настройки
                _config = new AppConfig { SavePath = "Game.saves/" };
                if (!Directory.Exists(_config.SavePath)) Directory.CreateDirectory(_config.SavePath);
                SaveConfig();
            }
        }

        // Запись конфига в Game.info
        private void SaveConfig() => _configSer.Serialize(ConfigPath, _config);

        // Подгрузка рекордов в таблицу на экране
        private void LoadHighScores()
        {
            GridScores.ItemsSource = _scoreManager.Load();
        }

        // Проверка: есть ли файл сохранения в Game.saves
        private void CheckSaveFile()
        {
            string ext = _config.UseXml ? ".xml" : ".json";
            string fullPath = Path.Combine(_config.SavePath, _saveFileName + ext);
            BtnContinue.IsEnabled = File.Exists(fullPath); // Включаем кнопку, если файл есть
        }

        // Кнопка "Новая игра": удаляет старую сессию
        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            string ext = _config.UseXml ? ".xml" : ".json";
            string fullPath = Path.Combine(_config.SavePath, _saveFileName + ext);
            if (File.Exists(fullPath)) File.Delete(fullPath); // Стираем старый сейв

            StartGame(null); // Запуск с нуля
        }

        // Кнопка "Продолжить"
        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            StartGame(_config.SavePath); // Загрузка из папки Game.saves
        }

        // Кнопка открытия настроек
        private void BtnSettings_Click(object sender, RoutedEventArgs e) => OpenSettings();

        // Показать окно настроек
        private void OpenSettings()
        {
            var settingsWin = new SettingsWindow(_config);
            settingsWin.Owner = this;
            if (settingsWin.ShowDialog() == true)
            {
                CheckSaveFile(); // Проверяем сейвы с новыми настройками
            }
        }

        // Переход в окно игры
        private void StartGame(string? loadPath)
        {
            MainWindow gameWindow = new MainWindow(loadPath, _config.UseXml, (int)SliderDifficulty.Value);
            gameWindow.Show();
            this.Close(); // Закрыть меню
        }

        // Полный выход из приложения
        private void BtnExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}