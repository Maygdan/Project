using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using Model.Core;
using Model.Core.Interfaces; 
using Model.Data;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using System.IO;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace DoodleJump.Views
{
    public partial class MainWindow : Window
    {
        private GameEngine _engine; // Движок игры
        private DateTime _lastFrameTime; // Время прошлого кадра
        private Dictionary<GameObject, UIElement> _visuals = new Dictionary<GameObject, UIElement>(); // Связь данных и картинок
        private HashSet<Key> _pressedKeys = new HashSet<Key>(); // Зажатые клавиши
        
        private string _savePath; // Папка сохранений (Game.saves)
        private bool _useXml; // Выбранный формат

        private double _forestOffset = 0; // Сдвиг леса
        private double _fogOffset = 0; // Сдвиг тумана
        private bool _isPaused = false; // Состояние паузы
        private List<Ellipse> _fireflies = new List<Ellipse>(); // Светлячки
        private Random _rnd = new Random();

        public MainWindow(string? loadPath, bool useXml, int difficulty)
        {
            InitializeComponent();
            
            _useXml = useXml;
            _savePath = loadPath ?? "Game.saves"; // Папка по умолчанию
            if (!Directory.Exists(_savePath)) Directory.CreateDirectory(_savePath);

            _engine = new GameEngine();
            _engine.Difficulty = difficulty;

            if (loadPath != null) LoadProgress(); // Загрузка сохранения

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
            this.Closing += (s, e) => { if (!_engine.IsGameOver) SaveProgress(); }; // Сохраняем, если не проиграли

            CreateFireflies(); 

            _lastFrameTime = DateTime.Now;
            CompositionTarget.Rendering += GameLoop;
        }

        // Создание светящихся частиц
        private void CreateFireflies()
        {
            for (int i = 0; i < 15; i++)
            {
                var f = new Ellipse {
                    Width = _rnd.Next(3, 6), Height = _rnd.Next(3, 6),
                    Fill = Brushes.LightYellow, Opacity = _rnd.NextDouble(),
                    Effect = new BlurEffect { Radius = 5 } 
                };
                Canvas.SetLeft(f, _rnd.Next(0, 500));
                Canvas.SetTop(f, _rnd.Next(0, 850));
                GameCanvas.Children.Add(f);
                _fireflies.Add(f);
            }
        }

        // Движение частиц вверх
        private void UpdateFireflies()
        {
            foreach (var f in _fireflies)
            {
                double top = Canvas.GetTop(f);
                Canvas.SetTop(f, top - 0.5); 
                if (top < -10) Canvas.SetTop(f, 860); 
            }
        }

        // Обработка кнопок без задержки
        private void HandleInput()
        {
            if (_isPaused) return;

            double speed = 360;
            double targetVelocity = 0;

            if (_pressedKeys.Contains(Key.A) || _pressedKeys.Contains(Key.Left)) targetVelocity = -speed;
            else if (_pressedKeys.Contains(Key.D) || _pressedKeys.Contains(Key.Right)) targetVelocity = speed;

            _engine.Player.VelocityX = targetVelocity;
        }

        // Игровой цикл
        private void GameLoop(object? sender, EventArgs e)
        {
            if (_isPaused || _engine.IsGameOver) 
            {
                if (_engine.IsGameOver) HandleGameOver();
                return;
            }

            HandleInput();

            var currentFrameTime = DateTime.Now;
            double deltaTime = (currentFrameTime - _lastFrameTime).TotalSeconds;
            _lastFrameTime = currentFrameTime;

            _engine.Update(deltaTime);

            if (TxtScore != null) TxtScore.Text = ((int)_engine.Score).ToString();

            // Эффект глубины фона
            if (_engine.Player.VelocityY < 0 && _engine.Player.Y <= GameEngine.CanvasHeight / 2)
            {
                ScrollBackground(Math.Abs(_engine.Player.VelocityY * deltaTime));
            }

            Draw();
        }

        // --- ЛОГИКА ОКОНЧАНИЯ ИГРЫ (БЕЗ MESSAGEBOX) ---

        private void HandleGameOver()
        {
            if (GameOverOverlay.Visibility == Visibility.Visible) return;

            _isPaused = true; 
            new ScoreManager().AddScore((int)_engine.Score, _engine.Difficulty); // Запись рекорда
            
            DeleteSave(); // ТЗ: Сгорание сессии (удаляем файл)

            // Показываем меню смерти
            GameOverOverlay.Visibility = Visibility.Visible;
            BtnPauseIcon.Visibility = Visibility.Collapsed;
            GameWorld.Effect = new BlurEffect { Radius = 20 }; // Сильное размытие мира
        }

        // Кнопка "ЗАНОВО" в меню смерти
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            MainWindow newGame = new MainWindow(null, _useXml, _engine.Difficulty);
            newGame.Show();
            this.Close();
        }

        // Кнопка "В МЕНЮ" (универсальная)
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            // Если игрок ЖИВ (вышел через паузу), сохраняем прогресс
            if (!_engine.IsGameOver) 
            {
                SaveProgress(); 
            }
            
            new StartWindow().Show(); 
            this.Close();
        }

        // Удаление сохранения после смерти
        private void DeleteSave()
        {
            string ext = _useXml ? ".xml" : ".json";
            string fullPath = System.IO.Path.Combine(_savePath, "savegame" + ext);
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);
        }

        // Параллакс фона
        private void ScrollBackground(double scrollY)
        {
            _forestOffset += scrollY * 0.001; 
            if (_forestOffset >= 850) _forestOffset = 0;
            Canvas.SetTop(Forest1, _forestOffset);
            Canvas.SetTop(Forest2, _forestOffset - 850);

            _fogOffset += scrollY * 0.0002; 
            if (_fogOffset >= 850) _fogOffset = 0;
            Canvas.SetTop(Fog1, _fogOffset);
            Canvas.SetTop(Fog2, _fogOffset - 850);
        }

        // Пауза
        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = true;
            PauseOverlay.Visibility = Visibility.Visible;
            BtnPauseIcon.Visibility = Visibility.Collapsed;
            GameWorld.Effect = new BlurEffect { Radius = 15 };
        }

        // Снятие паузы
        private void BtnResume_Click(object sender, RoutedEventArgs e)
        {
            _isPaused = false;
            PauseOverlay.Visibility = Visibility.Collapsed;
            BtnPauseIcon.Visibility = Visibility.Visible;
            GameWorld.Effect = null;
            _lastFrameTime = DateTime.Now;
        }

        // Настройки
        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var currentConfig = new AppConfig { SavePath = _savePath, UseXml = _useXml };
            var settingsWin = new SettingsWindow(currentConfig);
            settingsWin.Owner = this;
            if (settingsWin.ShowDialog() == true)
            {
                _savePath = currentConfig.SavePath;
                _useXml = currentConfig.UseXml;
                new JsonGameSerializer<AppConfig>().Serialize("config.json", currentConfig);
            }
        }

        // Рисование
        private void Draw()
        {
            foreach (Platform p in _engine.Platforms)
            {
                if (p.IsActive) UpdateOrCreateVisual(p, Brushes.Transparent);
            }
            UpdateOrCreateVisual(_engine.Player, Brushes.Transparent);
            UpdateFireflies();
            CleanupVisuals();
        }

        // Создание визуальных объектов
        private void UpdateOrCreateVisual(GameObject obj, Brush fallbackColor)
        {
            if (!_visuals.ContainsKey(obj))
            {
                Image img = new Image();
                string imageName = "short_platform.png"; 

                if (obj is Player) {
                    imageName = "Knight.png";
                    img.Width = 85; img.Height = 100;
                } else if (obj is TrapPlatform) imageName = "broken.png";
                else if (obj is SpringPlatform) imageName = "group3.png";
                else if (obj is Platform platBase) {
                    if (platBase.Width > 110) imageName = "long_platform.png";
                    else if (platBase.Width > 75) imageName = "middle_platform.png";
                }

                img.Source = new BitmapImage(new Uri($"/Assets/{imageName}", UriKind.Relative));
                img.Width = (obj is Player) ? 85 : (obj.Width * 1.5);
                img.Height = (obj is Player) ? 100 : (obj is SpringPlatform ? 150 : 130);
                
                _visuals[obj] = img;
                GameCanvas.Children.Add(img);
            }

            var visual = (Image)_visuals[obj];
            double offsetX = (visual.Width - obj.Width) / 2;

            if (obj is Player playerObj) {
                Canvas.SetLeft(visual, obj.X - offsetX);
                Canvas.SetTop(visual, obj.Y - (visual.Height - obj.Height) + 12); 
                if (playerObj.VelocityX < 0) visual.RenderTransform = new ScaleTransform(-1, 1); 
                else if (playerObj.VelocityX > 0) visual.RenderTransform = new ScaleTransform(1, 1);
                visual.RenderTransformOrigin = new Point(0.5, 0.5);
            } else {
                Canvas.SetLeft(visual, obj.X - offsetX);
                Canvas.SetTop(visual, obj.Y - 28); 
            }
        }

        // Удаление неактивных платформ
        private void CleanupVisuals()
        {
            var toRemove = _visuals.Keys
                .Where(obj => obj is Platform p && (!p.IsActive || !_engine.Platforms.Contains(p)))
                .ToList();
            foreach (var obj in toRemove)
            {
                GameCanvas.Children.Remove(_visuals[obj]);
                _visuals.Remove(obj);
            }
        }

        // Сохранение
        private void SaveProgress()
        {
            if (_engine.IsGameOver) return;
            var state = new GameState {
                PlayerX = _engine.Player.X, PlayerY = _engine.Player.Y,
                Score = _engine.Score, Difficulty = _engine.Difficulty,
                Platforms = _engine.Platforms.Select(p => new PlatformSaveData {
                    X = p.X, Y = p.Y, Type = p.GetType().Name
                }).ToList()
            };
            string fileName = System.IO.Path.Combine(_savePath, "savegame" + (_useXml ? ".xml" : ".json"));
            if (_useXml) new XmlGameSerializer<GameState>().Serialize(fileName, state);
            else new JsonGameSerializer<GameState>().Serialize(fileName, state);
        }

        // Загрузка
        private void LoadProgress()
        {
            string ext = _useXml ? ".xml" : ".json";
            string fullPath = System.IO.Path.Combine(_savePath, "savegame" + ext);
            if (!File.Exists(fullPath)) return;
            try {
                GameState state = _useXml 
                    ? new XmlGameSerializer<GameState>().Deserialize(fullPath)
                    : new JsonGameSerializer<GameState>().Deserialize(fullPath);
                _engine.LoadState(state);
                GameCanvas.Children.Clear();
                _visuals.Clear();
                Draw(); 
                _lastFrameTime = DateTime.Now;
            } catch (Exception ex) {
                MessageBox.Show($"Ошибка загрузки сессии: {ex.Message}");
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.Key)) _pressedKeys.Add(e.Key);
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.Key)) _pressedKeys.Remove(e.Key);
        }
    }
}