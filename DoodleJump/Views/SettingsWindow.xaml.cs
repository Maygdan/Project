using System.Windows;
using System.Windows.Controls;
using Model.Data;
using System.IO;
using Microsoft.Win32;

namespace DoodleJump.Views
{
    public partial class SettingsWindow : Window
    {
        private AppConfig _config;
        private JsonGameSerializer<AppConfig> _configSer = new();

        public SettingsWindow(AppConfig config)
        {
            InitializeComponent();
            _config = config;
            
            TxtSavePath.Text = _config.SavePath;
            ComboFormat.SelectedIndex = _config.UseXml ? 1 : 0;
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog { CheckFileExists = false, FileName = "Выбор папки" };
            if (dialog.ShowDialog() == true)
            {
                _config.SavePath = Path.GetDirectoryName(dialog.FileName) ?? "saves/";
                TxtSavePath.Text = _config.SavePath;
            }
        }

        private void ComboFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            _config.UseXml = ComboFormat.SelectedIndex == 1;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
{
    string oldPath = System.IO.Path.Combine(_config.SavePath, "savegame" + (_config.UseXml ? ".json" : ".xml"));
    string newPath = System.IO.Path.Combine(_config.SavePath, "savegame" + (_config.UseXml ? ".xml" : ".json"));

    // ТЗ: Копируем данные при смене формата
    if (File.Exists(oldPath) && !File.Exists(newPath))
    {
        try {
            GameState state = !_config.UseXml 
                ? new XmlGameSerializer<GameState>().Deserialize(oldPath)
                : new JsonGameSerializer<GameState>().Deserialize(oldPath);

            if (_config.UseXml) new XmlGameSerializer<GameState>().Serialize(newPath, state);
            else new JsonGameSerializer<GameState>().Serialize(newPath, state);
        } catch { }
    }

    _configSer.Serialize("config.json", _config);
    this.DialogResult = true;
    this.Close();
}
    }
}