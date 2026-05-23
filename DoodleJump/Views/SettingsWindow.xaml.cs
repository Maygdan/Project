using System.Windows;
using System.Windows.Controls;
using Model.Data;
using System.IO;
using Microsoft.Win32;

namespace DoodleJump.Views
{
    public partial class SettingsWindow : Window
    {
        private AppConfig _config; // Ссылка на данные настроек
        private JsonGameSerializer<AppConfig> _configSer = new(); // Инструмент сохранения конфига

        public SettingsWindow(AppConfig config) // Создание окна настроек
        {
            InitializeComponent();
            _config = config; // Запоминаем переданные настройки
            
            TxtSavePath.Text = _config.SavePath; // Показываем текущий путь
            ComboFormat.SelectedIndex = _config.UseXml ? 1 : 0; // Показываем текущий формат
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e) // Кнопка выбора папки
        {
            var dialog = new OpenFileDialog { CheckFileExists = false, FileName = "Выбор папки" };
            if (dialog.ShowDialog() == true)
            {
                _config.SavePath = Path.GetDirectoryName(dialog.FileName) ?? "saves/"; // Сохраняем новый путь
                TxtSavePath.Text = _config.SavePath; // Обновляем текст в поле
            }
        }

        private void ComboFormat_SelectionChanged(object sender, SelectionChangedEventArgs e) // Выбор JSON или XML
        {
            if (!IsLoaded) return;
            _config.UseXml = ComboFormat.SelectedIndex == 1; // Обновляем флаг использования XML
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e) // Кнопка "Сохранить и выйти"
        {
            // Пути для старого и нового форматов
            string oldPath = System.IO.Path.Combine(_config.SavePath, "savegame" + (_config.UseXml ? ".json" : ".xml"));
            string newPath = System.IO.Path.Combine(_config.SavePath, "savegame" + (_config.UseXml ? ".xml" : ".json"));

            // ТЗ: Копируем данные из одного формата в другой при смене настроек
            if (File.Exists(oldPath) && !File.Exists(newPath))
            {
                try {
                    GameState state = !_config.UseXml 
                        ? new XmlGameSerializer<GameState>().Deserialize(oldPath)
                        : new JsonGameSerializer<GameState>().Deserialize(oldPath);

                    if (_config.UseXml) new XmlGameSerializer<GameState>().Serialize(newPath, state);
                    else new JsonGameSerializer<GameState>().Serialize(newPath, state);
                } catch { } // Если файл поврежден, просто не копируем
            }

            _configSer.Serialize("config.json", _config); // Записываем конфиг на диск
            this.DialogResult = true; // Сигнал об успешном сохранении
            this.Close(); // Закрываем настройки
        }
    }
}